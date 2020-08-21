using Neo.IO.Json;

namespace Neo.Plugins
{
    public class BlockTransactionResponse
    {
        public Transaction Transaction { get; set; }

        public BlockTransactionResponse(Transaction transaction)
        {
            Transaction = transaction;
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["transaction"] = Transaction.ToJson();
            return json;
        }
    }
}
