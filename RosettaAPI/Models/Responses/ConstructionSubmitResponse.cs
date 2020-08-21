using Neo.IO.Json;

namespace Neo.Plugins
{
    public class ConstructionSubmitResponse
    {
        public TransactionIdentifier TransactionIdentifier { get; set; }
        public Metadata Metadata { get; set; }

        public ConstructionSubmitResponse(TransactionIdentifier transactionIdentifier, Metadata metadata = null)
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
