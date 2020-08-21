using Neo.IO.Json;

namespace Neo.Plugins
{
    public class SubAccountIdentifier
    {
        // nep5 related contract address
        public string Address { get; set; }
        public Metadata Metadata { get; set; }

        public SubAccountIdentifier(string address, Metadata metadata = null)
        {
            Address = address;
            Metadata = metadata;
        }

        public static SubAccountIdentifier FromJson(JObject json)
        {
            return new SubAccountIdentifier(json["address"].AsString(),
                json.ContainsProperty("metadata") ? Metadata.FromJson(json["metadata"]) : null);
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["address"] = Address;
            if (Metadata != null && Metadata.ToJson() != null)
                json["metadata"] = Metadata.ToJson();
            return json;
        }
    }
}
