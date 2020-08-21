using Neo.IO.Json;

namespace Neo.Plugins
{
    public class NetworkRequest
    {
        public NetworkIdentifier NetworkIdentifier { get; set; }
        public Metadata Metadata { get; set; }

        public NetworkRequest(NetworkIdentifier networkIdentifier, Metadata metadata = null)
        {
            NetworkIdentifier = networkIdentifier;
            Metadata = metadata;
        }

        public static NetworkRequest FromJson(JObject json)
        {
            return new NetworkRequest(NetworkIdentifier.FromJson(json["network_identifier"]),
                json.ContainsProperty("metadata") ? Metadata.FromJson(json["metadata"]) : null);
        }
    }
}
