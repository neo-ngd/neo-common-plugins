using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.DependencyInjection;
using Neo.IO;
using Neo.IO.Data.LevelDB;
using Neo.IO.Json;
using Neo.Ledger;
using Neo.Persistence;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static System.IO.Path;
using NeoBlock = Neo.Network.P2P.Payloads.Block;

namespace Neo.Plugins
{
    public sealed class N3RosettaAPI : Plugin, IPersistencePlugin
    {
        private IWebHost host;
        private RosettaController controller;
        private DB db;
        private WriteBatch _writeBatch;

        public override string Name => "N3RosettaAPI";

        protected override void Configure()
        {
            Settings.Load(GetConfiguration());
            string path = string.Format(Settings.Default.DBPath, Settings.Default.Network.ToString("X8"));
            db = DB.Open(GetFullPath(path), new Options { CreateIfMissing = true });
        }

        protected override void OnSystemLoaded(NeoSystem system)
        {
            if (system.Settings.Network != Settings.Default.Network) return;

            controller = new RosettaController(system, this.db);
            var dflt = Settings.Default;
            host = new WebHostBuilder().UseKestrel(options => options.Listen(dflt.BindAddress, dflt.Port, listenOptions =>
            {
                // default is unlimited
                options.Limits.MaxConcurrentConnections = 50;
                // default is 2 minutes
                options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(1);
                // default is 30 seconds
                options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(15);

                if (string.IsNullOrEmpty(dflt.SslCert)) return;
                listenOptions.UseHttps(dflt.SslCert, dflt.SslCertPassword, httpsConnectionAdapterOptions =>
                {
                    if (dflt.TrustedAuthorities is null || dflt.TrustedAuthorities.Length == 0)
                        return;
                    httpsConnectionAdapterOptions.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
                    httpsConnectionAdapterOptions.ClientCertificateValidation = (cert, chain, err) =>
                    {
                        if (err != SslPolicyErrors.None)
                            return false;
                        X509Certificate2 authority = chain.ChainElements[chain.ChainElements.Count - 1].Certificate;
                        return dflt.TrustedAuthorities.Contains(authority.Thumbprint);
                    };
                });
            }))
            .ConfigureServices(services =>
            {
                services.AddResponseCompression(options =>
                {
                    // options.EnableForHttps = false;
                    options.Providers.Add<GzipCompressionProvider>();
                    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json" });
                });

                services.Configure<GzipCompressionProviderOptions>(options =>
                {
                    options.Level = CompressionLevel.Fastest;
                });
            })
            .Configure(app =>
            {
                app.UseResponseCompression();
                app.Run(ProcessAsync);
            })
            .Build();
            host.Start();
        }

        void IPersistencePlugin.OnPersist(NeoSystem system, NeoBlock block, DataCache snapshot, IReadOnlyList<Blockchain.ApplicationExecuted> applicationExecutedList)
        {
            if (system.Settings.Network != Settings.Default.Network) return;

            ResetBatch();

            //processing log for transactions
            foreach (var appExec in applicationExecutedList.Where(p => p.Transaction != null))
            {
                var txJson = TxLogToJson(appExec);
                Put(appExec.Transaction.Hash.ToArray(), Neo.Utility.StrictUTF8.GetBytes(txJson.ToString()));
            }

            //processing log for block
            var blockJson = BlockLogToJson(block, applicationExecutedList);
            if (blockJson != null)
            {
                Put(block.Hash.ToArray(), Neo.Utility.StrictUTF8.GetBytes(blockJson.ToString()));
            }
        }

        public static JObject TxLogToJson(Blockchain.ApplicationExecuted appExec)
        {
            global::System.Diagnostics.Debug.Assert(appExec.Transaction != null);

            var txJson = new JObject();
            txJson["txid"] = appExec.Transaction.Hash.ToString();
            JObject trigger = new JObject();
            trigger["trigger"] = appExec.Trigger;
            trigger["vmstate"] = appExec.VMState;
            trigger["exception"] = GetExceptionMessage(appExec.Exception);
            trigger["gasconsumed"] = appExec.GasConsumed.ToString();
            try
            {
                trigger["stack"] = appExec.Stack.Select(q => q.ToJson()).ToArray();
            }
            catch (InvalidOperationException)
            {
                trigger["stack"] = "error: recursive reference";
            }
            trigger["notifications"] = appExec.Notifications.Select(q =>
            {
                JObject notification = new JObject();
                notification["contract"] = q.ScriptHash.ToString();
                notification["eventname"] = q.EventName;
                try
                {
                    notification["state"] = q.State.ToJson();
                }
                catch (InvalidOperationException)
                {
                    notification["state"] = "error: recursive reference";
                }
                return notification;
            }).ToArray();

            txJson["executions"] = new List<JObject>() { trigger }.ToArray();
            return txJson;
        }

        public static JObject BlockLogToJson(NeoBlock block, IReadOnlyList<Blockchain.ApplicationExecuted> applicationExecutedList)
        {
            var blocks = applicationExecutedList.Where(p => p.Transaction is null).ToArray();
            if (blocks.Length > 0)
            {
                var blockJson = new JObject();
                var blockHash = block.Hash.ToArray();
                blockJson["blockhash"] = block.Hash.ToString();
                var triggerList = new List<JObject>();
                foreach (var appExec in blocks)
                {
                    JObject trigger = new JObject();
                    trigger["trigger"] = appExec.Trigger;
                    trigger["vmstate"] = appExec.VMState;
                    trigger["gasconsumed"] = appExec.GasConsumed.ToString();
                    try
                    {
                        trigger["stack"] = appExec.Stack.Select(q => q.ToJson()).ToArray();
                    }
                    catch (InvalidOperationException)
                    {
                        trigger["stack"] = "error: recursive reference";
                    }
                    trigger["notifications"] = appExec.Notifications.Select(q =>
                    {
                        JObject notification = new JObject();
                        notification["contract"] = q.ScriptHash.ToString();
                        notification["eventname"] = q.EventName;
                        try
                        {
                            notification["state"] = q.State.ToJson();
                        }
                        catch (InvalidOperationException)
                        {
                            notification["state"] = "error: recursive reference";
                        }
                        return notification;
                    }).ToArray();
                    triggerList.Add(trigger);
                }
                blockJson["executions"] = triggerList.ToArray();
                return blockJson;
            }

            return null;
        }


        void IPersistencePlugin.OnCommit(NeoSystem system, NeoBlock block, DataCache snapshot)
        {
            if (system.Settings.Network != Settings.Default.Network) return;
            db.Write(WriteOptions.Default, _writeBatch);
        }

        static string GetExceptionMessage(Exception exception)
        {
            return exception?.GetBaseException().Message;
        }

        private void ResetBatch()
        {
            _writeBatch = new WriteBatch();
        }

        private void Put(byte[] key, byte[] value)
        {
            _writeBatch.Put(key, value);
        }


        private async Task ProcessAsync(HttpContext context)
        {
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            context.Response.Headers["Access-Control-Allow-Methods"] = "POST";
            context.Response.Headers["Access-Control-Allow-Headers"] = "Content-Type";
            context.Response.Headers["Access-Control-Max-Age"] = "31536000";
            if (context.Request.Method != "POST") return;

            JObject request = null;
            using StreamReader reader = new(context.Request.Body);
            try
            {
                request = JObject.Parse(await reader.ReadToEndAsync());
            }
            catch (FormatException) { }

            JObject response;
            if (request is null)
                response = Error.PARSE_REQUEST_ERROR.ToJson();
            else
            {
                // get path
                string path = context.Request.Path.Value;

                try
                {
                    response = Process(path, request);
                }
                catch (Exception ex)
                {
                    response = new Error(0, ex.Message).ToJson();
                }
            }
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(response.ToString(), Encoding.UTF8);
        }

        private JObject Process(string path, JObject request)
        {
            switch (path)
            {
                case "/network/list":
                    return controller.NetworkList(MetadataRequest.FromJson(request));
                case "/network/options":
                    return controller.NetworkOptions(NetworkRequest.FromJson(request));
                case "/network/status":
                    return controller.NetworkStatus(NetworkRequest.FromJson(request));
                case "/account/balance":
                    return controller.AccountBalance(AccountBalanceRequest.FromJson(request));
                case "/block":
                    return controller.Block(BlockRequest.FromJson(request));
                case "/block/transaction":
                    return controller.BlockTransaction(BlockTransactionRequest.FromJson(request));
                case "/mempool":
                    return controller.Mempool(NetworkRequest.FromJson(request));
                case "/mempool/transaction":
                    return controller.MempoolTransaction(MempoolTransactionRequest.FromJson(request));
                case "/construction/combine":
                    return controller.ConstructionCombine(ConstructionCombineRequest.FromJson(request));
                case "/construction/derive":
                    return controller.ConstructionDerive(ConstructionDeriveRequest.FromJson(request));
                case "/construction/hash":
                    return controller.ConstructionHash(ConstructionHashRequest.FromJson(request));
                case "/construction/metadata":
                    return controller.ConstructionMetadata(ConstructionMetadataRequest.FromJson(request));
                case "/construction/parse":
                    return controller.ConstructionParse(ConstructionParseRequest.FromJson(request));
                case "/construction/payloads":
                    return controller.ConstructionPayloads(ConstructionPayloadsRequest.FromJson(request));
                case "/construction/preprocess":
                    return controller.ConstructionPreprocess(ConstructionPreprocessRequest.FromJson(request));
                case "/construction/submit":
                    return controller.ConstructionSubmit(ConstructionSubmitRequest.FromJson(request));
                default:
                    throw new ArgumentException("path is not supported");
            }
        }
    }
}
