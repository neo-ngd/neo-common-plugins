using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Neo.Plugins
{
    public class GraphQLServer: Plugin 
    {
        private IWebHost host;

        public override void Dispose()
        {
            base.Dispose();
            if (host != null)
            {
                host.Dispose();
                host = null;
            }
        }

        protected override void Configure()
        {
            GraphSettings.Load(GetConfiguration());
        }

        protected override void OnPluginsLoaded()
        {
            host = new WebHostBuilder().UseKestrel(options => options.Listen(GraphSettings.Default.BindAddress, GraphSettings.Default.Port, listenOptions =>
            {
                if (string.IsNullOrEmpty(GraphSettings.Default.SslCert)) return;
                listenOptions.UseHttps(GraphSettings.Default.SslCert, GraphSettings.Default.SslCertPassword, httpsConnectionAdapterOptions =>
                {
                    if (GraphSettings.Default.TrustedAuthorities is null || GraphSettings.Default.TrustedAuthorities.Length == 0)
                        return;
                    httpsConnectionAdapterOptions.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
                    httpsConnectionAdapterOptions.ClientCertificateValidation = (cert, chain, err) =>
                    {
                        if (err != SslPolicyErrors.None)
                            return false;
                        X509Certificate2 authority = chain.ChainElements[chain.ChainElements.Count - 1].Certificate;
                        return GraphSettings.Default.TrustedAuthorities.Contains(authority.Thumbprint);
                    };
                });
            }))
               .ConfigureServices(services =>
               {
                   services.AddScoped<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));
                   services.AddScoped<IGraphService, GraphService>(s => new GraphService(System));
                   services.AddScoped<RootSchema>();
                   services.AddGraphQL().AddGraphTypes(ServiceLifetime.Scoped);
                   services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
               })
            .Configure(app =>
            {
                app.UseGraphQL<RootSchema>();
                app.UseGraphQLPlayground(new GraphQLPlaygroundOptions());
                app.UseMvc();
            })
            .Build();
            host.Start();
        }
    }
}
