using Neo.IO.Json;

namespace Neo.Plugins
{
    // ConstructionParseRequest is the input to the /construction/parse endpoint.
    // It allows the caller to parse either an unsigned or signed transaction.
    public class ConstructionParseRequest
    {
        // The network_identifier specifies which network a particular object is associated with.
        public NetworkIdentifier NetworkIdentifier { get; set; }
        // Signed is a boolean indicating whether the transaction is signed.
        public bool Signed { get; set; }
        // 	This must be either the unsigned transaction blob returned by /construction/payloads
        // 	or the signed transaction blob returned by /construction/combine.
        public string Transaction { get; set; } // base64 string

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
