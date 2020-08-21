using Neo.IO.Json;

namespace Neo.Plugins
{
    // The Version object is utilized to inform the client of the versions of different
    // components of the Rosetta implementation.
    public class Version
    {
        public string RosettaVersion { get; set; }
        public string NodeVersion { get; set; }
        public string MiddlewareVersion { get; set; }
        public Metadata Metadata { get; set; }

        public Version(string rosettaVersion, string nodeVersion, string middlewareVersion = null, Metadata metadata = null)
        {
            RosettaVersion = rosettaVersion;
            NodeVersion = nodeVersion;
            MiddlewareVersion = middlewareVersion;
            Metadata = metadata;
        }


        public JObject ToJson()
        {
            JObject json = new JObject();
            json["rosetta_version"] = RosettaVersion;
            json["node_version"] = NodeVersion;
            if (!string.IsNullOrEmpty(MiddlewareVersion))
                json["middleware_version"] = MiddlewareVersion;
            if (Metadata != null && Metadata.ToJson() != null)
                json["metadata"] = Metadata.ToJson();
            return json;
        }
    }
}
