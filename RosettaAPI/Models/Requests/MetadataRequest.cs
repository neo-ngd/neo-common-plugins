using Neo.IO.Json;

namespace Neo.Plugins
{
    public class MetadataRequest
    {
        public Metadata Metadata { get; set; }

        public MetadataRequest(Metadata metadata = null)
        {
            Metadata = metadata;
        }

        public static MetadataRequest FromJson(JObject json)
        {
            return new MetadataRequest(Metadata.FromJson(json["metadata"]));
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["metadata"] = Metadata?.ToJson();
            return json;
        }
    }
}
