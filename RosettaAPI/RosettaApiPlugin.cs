using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.DependencyInjection;
using Neo.IO.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Neo.Plugins
{
    public sealed class RosettaApiPlugin : Plugin
    {
        private IWebHost host;
        private RosettaController controller;

        public override void Configure()
        {
            RosettaApiSettings.Load(GetConfiguration());
        }

        protected override void OnPluginsLoaded()
        {
            controller = new RosettaController(System);
            var dflt = RosettaApiSettings.Default;
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

        private async Task ProcessAsync(HttpContext context)
        {
            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            context.Response.Headers["Access-Control-Allow-Methods"] = "POST";
            context.Response.Headers["Access-Control-Allow-Headers"] = "Content-Type";
            context.Response.Headers["Access-Control-Max-Age"] = "31536000";
            if (context.Request.Method != "POST") return;

            JObject request = null;
            using (StreamReader reader = new StreamReader(context.Request.Body))
            {
                try
                {
                    request = JObject.Parse(reader);
                }
                catch (FormatException) { }
            }
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
