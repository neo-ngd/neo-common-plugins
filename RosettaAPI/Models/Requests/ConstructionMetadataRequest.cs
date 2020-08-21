using Neo.IO.Json;

namespace Neo.Plugins
{
    public class ConstructionMetadataRequest
    {
        public NetworkIdentifier NetworkIdentifier { get; set; }
        public Metadata Options { get; set; }

        public ConstructionMetadataRequest(NetworkIdentifier networkIdentifier, Metadata options)
        {
            NetworkIdentifier = networkIdentifier;
            Options = options;
        }

        public static ConstructionMetadataRequest FromJson(JObject json)
        {
            return new ConstructionMetadataRequest(NetworkIdentifier.FromJson(json["network_identifier"]),
                Metadata.FromJson(json["options"]));
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["network_identifier"] = NetworkIdentifier.ToJson();
            json["options"] = Options.ToJson();
            return json;
        }
    }
}
