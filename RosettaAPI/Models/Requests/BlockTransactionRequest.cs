using Neo.IO.Json;

namespace Neo.Plugins
{
    public class BlockTransactionRequest
    {
        public NetworkIdentifier NetworkIdentifier { get; set; }
        public BlockIdentifier BlockIdentifier { get; set; }
        public TransactionIdentifier TransactionIdentifier { get; set; }

        public BlockTransactionRequest(NetworkIdentifier networkIdentifier, BlockIdentifier blockIdentifier, TransactionIdentifier transactionIdentifier)
        {
            NetworkIdentifier = networkIdentifier;
            BlockIdentifier = blockIdentifier;
            TransactionIdentifier = transactionIdentifier;
        }

        public static BlockTransactionRequest FromJson(JObject json)
        {
            return new BlockTransactionRequest(NetworkIdentifier.FromJson(json["network_identifier"]),
                BlockIdentifier.FromJson(json["block_identifier"]),
                TransactionIdentifier.FromJson(json["transaction_identifier"]));
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["network_identifier"] = NetworkIdentifier.ToJson();
            json["block_identifier"] = BlockIdentifier.ToJson();
            json["transaction_identifier"] = TransactionIdentifier.ToJson();
            return json;
        }
    }
}
