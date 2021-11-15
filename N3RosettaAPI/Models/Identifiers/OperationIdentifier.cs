using Neo.IO.Json;

namespace Neo.Plugins
{
    // OperationIdentifier uniquely identifies an operation within a transaction.
    public class OperationIdentifier
    {
        // The operation index is used to ensure each operation has a unique identifier within a
        // transaction. This index is only relative to the transaction and NOT GLOBAL. The operations in
        // each transaction should start from index 0. To clarify, there may not be any notion of an
        // operation index in the blockchain being described.
        public long Index { get; set; }

        // Some blockchains specify an operation index that is essential for client use. For example,
        // Bitcoin uses a network_index to identify which UTXO was used in a transaction. network_index
        // should not be populated if there is no notion of an operation index in a blockchain
        // (typically most account-based blockchains).
        public long? NetworkIndex { get; set; }

        public OperationIdentifier(long index, long? networkIndex = null)
        {
            Index = index;
            NetworkIndex = networkIndex;
        }

        public static OperationIdentifier FromJson(JObject json)
        {
            return new OperationIdentifier((long)json["index"].AsNumber(),
                json.ContainsProperty("network_index") ? (long?)json["network_index"].AsNumber() : null);
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["index"] = Index.ToString();
            if (NetworkIndex != null)
                json["network_index"] = NetworkIndex.ToString();
            return json;
        }
    }
}
