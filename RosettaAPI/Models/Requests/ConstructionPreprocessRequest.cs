using Neo.IO.Json;
using System.Linq;

namespace Neo.Plugins
{
    public class ConstructionPreprocessRequest
    {
        public NetworkIdentifier NetworkIdentifier { get; set; }
        public Operation[] Operations { get; set; }
        public Metadata Metadata { get; set; }

        public ConstructionPreprocessRequest(NetworkIdentifier networkIdentifier, Operation[] operations, Metadata metadata = null)
        {
            NetworkIdentifier = networkIdentifier;
            Operations = operations;
            Metadata = metadata;
        }

        public static ConstructionPreprocessRequest FromJson(JObject json)
        {
            return new ConstructionPreprocessRequest(NetworkIdentifier.FromJson(json["network_identifier"]),
                (json["operations"] as JArray).Select(p => Operation.FromJson(p)).ToArray(),
                json.ContainsProperty("metadata") ? Metadata.FromJson(json["metadata"]) : null);
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["network_identifier"] = NetworkIdentifier.ToJson();
            json["operations"] = Operations.Select(p => p.ToJson()).ToArray();
            if (Metadata != null && Metadata.ToJson() != null)
                json["metadata"] = Metadata.ToJson();
            return json;
        }
    }
}
