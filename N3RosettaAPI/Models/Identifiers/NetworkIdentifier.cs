using Neo.IO.Json;

namespace Neo.Plugins
{
    public class NetworkIdentifier
    {
        public string Blockchain { get; set; }
        public string Network { get; set; }
        public SubNetworkIdentifier SubNetworkIdentifier { get; set; }

        public NetworkIdentifier(string blockChain, string network, SubNetworkIdentifier subNetworkIdentifier = null)
        {
            Blockchain = blockChain;
            Network = network;
            SubNetworkIdentifier = subNetworkIdentifier;
        }

        public static NetworkIdentifier FromJson(JObject json)
        {
            return new NetworkIdentifier(json["blockchain"].AsString(),
                json["network"].AsString(),
                json.ContainsProperty("sub_network_identifier") ? SubNetworkIdentifier.FromJson(json["sub_network_identifier"]) : null);
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["blockchain"] = Blockchain;
            json["network"] = Network;
            if (SubNetworkIdentifier != null)
                json["sub_network_identifier"] = SubNetworkIdentifier.ToJson();
            return json;
        }
    }
}
