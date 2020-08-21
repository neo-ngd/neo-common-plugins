using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Net;

namespace Neo.Plugins
{
    internal class RosettaApiSettings
    {
        public string RosettaVersion { get; }
        public IPAddress BindAddress { get; }
        public ushort Port { get; }
        public string SslCert { get; }
        public string SslCertPassword { get; }
        public string[] TrustedAuthorities { get; }

        public static RosettaApiSettings Default { get; private set; }

        private RosettaApiSettings(IConfigurationSection section)
        {
            this.RosettaVersion = section.GetSection("RosettaVersion").Value;
            this.BindAddress = IPAddress.Parse(section.GetSection("BindAddress").Value);
            this.Port = ushort.Parse(section.GetSection("Port").Value);
            this.SslCert = section.GetSection("SslCert").Value;
            this.SslCertPassword = section.GetSection("SslCertPassword").Value;
            this.TrustedAuthorities = section.GetSection("TrustedAuthorities").GetChildren().Select(p => p.Get<string>()).ToArray();
        }

        public static void Load(IConfigurationSection section)
        {
            Default = new RosettaApiSettings(section);
        }
    }
}
