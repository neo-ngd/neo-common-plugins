using Neo.IO.Json;

namespace Neo.Plugins
{
    public class MempoolTransactionResponse
    {
        public Transaction Transaction { get; set; }
        public Metadata Metadata { get; set; }

        public MempoolTransactionResponse(Transaction transaction, Metadata metadata = null)
        {
            Transaction = transaction;
            Metadata = metadata;
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["transaction"] = Transaction.ToJson();
            if (Metadata != null && Metadata.ToJson() != null)
                json["metadata"] = Metadata.ToJson();
            return json;
        }
    }
}