using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Net;

namespace Neo.Plugins
{
    public class Settings
    {
        public uint Network { get; }
        public string DBPath { get; }
        public string RosettaVersion { get; }
        public bool EnableHistoricalBalance { get; }
        public IPAddress BindAddress { get; }
        public ushort Port { get; }
        public string SslCert { get; }
        public string SslCertPassword { get; }
        public string[] TrustedAuthorities { get; }

        public static Settings Default { get; private set; }

        private Settings(IConfigurationSection section)
        {
            this.Network = section.GetValue("Network", 5195086u);
            this.DBPath = section.GetValue("DBPath", "RosettaAPI_{0}");
            this.RosettaVersion = section.GetSection("RosettaVersion").Value;
            this.EnableHistoricalBalance = section.GetValue("EnableHistoricalBalance", true);
            this.BindAddress = IPAddress.Parse(section.GetSection("BindAddress").Value);
            this.Port = ushort.Parse(section.GetSection("Port").Value);
            this.SslCert = section.GetSection("SslCert").Value;
            this.SslCertPassword = section.GetSection("SslCertPassword").Value;
            this.TrustedAuthorities = section.GetSection("TrustedAuthorities").GetChildren().Select(p => p.Get<string>()).ToArray();
        }

        public static void Load(IConfigurationSection section)
        {
            Default = new Settings(section);
        }
    }
}
