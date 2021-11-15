using Neo.IO.Json;

namespace Neo.Plugins
{
    public class ConstructionHashRequest
    {
        public NetworkIdentifier NetworkIdentifier { get; set; }
        public string SignedTransaction { get; set; }

        public ConstructionHashRequest(NetworkIdentifier networkIdentifier, string signedTransaction)
        {
            NetworkIdentifier = networkIdentifier;
            SignedTransaction = signedTransaction;
        }

        public static ConstructionHashRequest FromJson(JObject json)
        {
            return new ConstructionHashRequest(NetworkIdentifier.FromJson(json["network_identifier"]),
                json["signed_transaction"].AsString());
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["network_identifier"] = NetworkIdentifier.ToJson();
            json["signed_transaction"] = SignedTransaction;
            return json;
        }
    }
}
