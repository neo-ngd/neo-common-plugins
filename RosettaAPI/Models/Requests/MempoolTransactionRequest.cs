using Neo.IO.Json;

namespace Neo.Plugins
{
    public class MempoolTransactionRequest
    {
        public NetworkIdentifier NetworkIdentifier { get; set; }
        public TransactionIdentifier TransactionIdentifier { get; set; }

        public MempoolTransactionRequest(NetworkIdentifier networkIdentifier, TransactionIdentifier transactionIdentifier)
        {
            NetworkIdentifier = networkIdentifier;
            TransactionIdentifier = transactionIdentifier;
        }

        public static MempoolTransactionRequest FromJson(JObject json)
        {
            return new MempoolTransactionRequest(NetworkIdentifier.FromJson(json["network_identifier"]),
                TransactionIdentifier.FromJson(json["transaction_identifier"]));
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["network_identifier"] = NetworkIdentifier.ToJson();
            json["transaction_identifier"] = TransactionIdentifier.ToJson();
            return json;
        }
    }
}
