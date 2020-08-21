using Neo.IO.Json;

namespace Neo.Plugins
{
    public class ConstructionDeriveResponse
    {
        public string Address { get; set; }
        public Metadata Metadata { get; set; }

        public ConstructionDeriveResponse(string address, Metadata metadata = null)
        {
            Address = address;
            Metadata = metadata;
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
