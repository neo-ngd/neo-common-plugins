using Neo.IO.Json;
using System.Linq;

namespace Neo.Plugins
{
    public class NetworkListResponse
    {
        public NetworkIdentifier[] NetworkIdentifiers { get; set; }

        public NetworkListResponse(NetworkIdentifier[] networkIdentifiers)
        {
            NetworkIdentifiers = networkIdentifiers;
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["network_identifiers"] = NetworkIdentifiers.Select(p => p.ToJson()).ToArray();
            return json;
        }
    }
}
