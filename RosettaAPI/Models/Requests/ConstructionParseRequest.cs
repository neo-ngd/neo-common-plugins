using Neo.IO.Json;

namespace Neo.Plugins
{
    public class ConstructionParseRequest
    {
        public NetworkIdentifier NetworkIdentifier { get; set; }
        public bool Signed { get; set; }
        public string Transaction { get; set; }

        public ConstructionParseRequest(NetworkIdentifier networkIdentifier, bool signed, string transaction)
        {
            NetworkIdentifier = networkIdentifier;
            Signed = signed;
            Transaction = transaction;
        }

        public static ConstructionParseRequest FromJson(JObject json)
        {
            return new ConstructionParseRequest(NetworkIdentifier.FromJson(json["network_identifier"]),
                json["signed"].AsBoolean(),
                json["transaction"].AsString());
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["network_identifier"] = NetworkIdentifier.ToJson();
            json["signed"] = Signed.ToString().ToLower();
            json["transaction"] = Transaction;
            return json;
        }
    }
}
