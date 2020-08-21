using Neo.IO.Json;
using System.Linq;

namespace Neo.Plugins
{
    // Operations contain all balance-changing information within a transaction. They are
    // always one-sided (only affect 1 AccountIdentifier) and can succeed or fail independently from a
    // Transaction.
    public class Operation
    {
        public OperationIdentifier OperationIdentifier { get; set; }

        // Restrict referenced related_operations to identifier indexes < the current
        // operation_identifier.index. This ensures there exists a clear DAG-structure of relations.
        // Since operations are one-sided, one could imagine relating operations in a single transfer or
        // linking operations in a call tree.
        public OperationIdentifier[] RelatedOperations { get; set; }

        // The network-specific type of the operation. Ensure that any type that can be returned here is
        // also specified in the NetworkStatus. This can be very useful to downstream consumers that
        // parse all block data.
        public string Type { get; set; }

        // The network-specific status of the operation. Status is not defined on the transaction object
        // because blockchains with smart contracts may have transactions that partially apply.
        // Blockchains with atomic transactions (all operations succeed or all operations fail) will
        // have the same status for each operation.
        public string Status { get; set; }
        public AccountIdentifier Account { get; set; }
        public Amount Amount { get; set; }

        // CoinChange is used to represent a change in state of a some coin identified by a coin_identifier. 
        // This object is part of the Operation model and must be populated for UTXO-based blockchains.
        public CoinChange CoinChange { get; set; }
        public Metadata Metadata { get; set; }

        public Operation(OperationIdentifier operationIdentifier, string type, string status, 
            OperationIdentifier[] relatedOperations = null, AccountIdentifier account = null, 
            Amount amount = null, CoinChange coinChange = null, Metadata metadata = null)
        {
            OperationIdentifier = operationIdentifier;
            RelatedOperations = relatedOperations;
            Type = type;
            Status = status;
            Account = account;
            Amount = amount;
            CoinChange = coinChange;
            Metadata = metadata;
        }

        public static Operation FromJson(JObject json)
        {
            return new Operation(OperationIdentifier.FromJson(json["operation_identifier"]),
                json["type"].AsString(),
                json["status"].AsString(),
                json.ContainsProperty("related_operations") ? (json["related_operations"] as JArray).Select(p => OperationIdentifier.FromJson(p)).ToArray() : null,
                json.ContainsProperty("account") ? AccountIdentifier.FromJson(json["account"]) : null,
                json.ContainsProperty("amount") ? Amount.FromJson(json["amount"]) : null,
                json.ContainsProperty("coin_change") ? CoinChange.FromJson(json["coin_change"]) : null,
                json.ContainsProperty("metadata") ? Metadata.FromJson(json["metadata"]) : null);
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["operation_identifier"] = OperationIdentifier.ToJson();
            if (RelatedOperations != null)
                json["related_operations"] = RelatedOperations.Select(p => p.ToJson()).ToArray();
            json["type"] = Type;
            json["status"] = Status;
            if (Account != null)
                json["account"] = Account.ToJson();
            if (Amount != null)
                json["amount"] = Amount.ToJson();
            if (CoinChange != null)
                json["coin_change"] = CoinChange.ToJson();
            if (Metadata != null && Metadata.ToJson() != null)
                json["metadata"] = Metadata.ToJson();
            return json;
        }
    }
}
