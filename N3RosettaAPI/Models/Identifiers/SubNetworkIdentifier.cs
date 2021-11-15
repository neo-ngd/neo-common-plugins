using Neo.IO.Json;

namespace Neo.Plugins
{
    public class SubNetworkIdentifier
    {
        public string Network { get; set; }
        public Metadata Metadata { get; set; }

        public SubNetworkIdentifier(string network, Metadata metadata = null)
        {
            Network = network;
            Metadata = metadata;
        }

        public static SubNetworkIdentifier FromJson(JObject json)
        {
            return new SubNetworkIdentifier(json["network"].AsString(),
                json.ContainsProperty("metadata") ? Metadata.FromJson(json["metadata"]) : null);
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["network"] = Network;
            if (Metadata != null && Metadata.ToJson() != null)
                json["metadata"] = Metadata.ToJson();
            return json;
        }
    }
}
