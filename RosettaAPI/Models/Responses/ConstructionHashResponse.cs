using Neo.IO.Json;

namespace Neo.Plugins
{
    public class ConstructionHashResponse
    {
        public string TransactionHash { get; set; }

        public ConstructionHashResponse(string transactionHash)
        {
            TransactionHash = transactionHash;
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["transaction_hash"] = TransactionHash;
            return json;
        }
    }
}
