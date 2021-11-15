using Neo.IO.Json;
using System.Linq;

namespace Neo.Plugins
{
    // Transactions contain an array of Operations that are attributable to the same
    // TransactionIdentifier.
    public class Transaction
    {
        public TransactionIdentifier TransactionIdentifier { get; set; }
        public Operation[] Operations { get; set; }
        public Metadata Metadata { get; set; }

        public Transaction(TransactionIdentifier transactionIdentifier, Operation[] operations, Metadata metadata = null)
        {
            TransactionIdentifier = transactionIdentifier;
            Operations = operations;
            Metadata = metadata;
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["transaction_identifier"] = TransactionIdentifier.ToJson();
            json["operations"] = Operations.Select(p => p.ToJson()).ToArray();
            if (Metadata != null && Metadata.ToJson() != null)
                json["metadata"] = Metadata.ToJson();
            return json;
        }
    }
}
