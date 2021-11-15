using Neo.IO.Json;

namespace Neo.Plugins
{
    // ConstructionHashResponse contains the transaction_identifier of a transaction that was submitted to /construction/hash.
    public class ConstructionHashResponse
    {
        // The transaction_identifier uniquely identifies a transaction in a particular network and block or in the mempool.
        public TransactionIdentifier TransactionIdentifier { get; set; }
        public Metadata Metadata { get; set; }

        public ConstructionHashResponse(TransactionIdentifier transactionIdentifier, Metadata metadata = null)
        {
            TransactionIdentifier = transactionIdentifier;
            Metadata = metadata;
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["transaction_identifier"] = TransactionIdentifier.ToJson();
            if (Metadata != null && Metadata.ToJson() != null)
                json["metadata"] = Metadata.ToJson();
            return json;
        }
    }
}
