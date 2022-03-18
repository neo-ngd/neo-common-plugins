using Neo.IO.Json;

namespace Neo.Plugins
{
    // TransactionIdentifier uniquely identifies a transaction in a
    // particular network and block or in the mempool.
    public class TransactionIdentifier
    {
        // Any transactions that are attributable only to a block (ex: a block event) should use the
        // hash of the block as the identifier.
        public string Hash { get; set; }

        public TransactionIdentifier(string hash)
        {
            Hash = hash;
        }

        public static TransactionIdentifier FromJson(JObject json)
        {
            if (json is null) return null;
            return new TransactionIdentifier(json["hash"]?.AsString());
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["hash"] = Hash;
            return json;
        }
    }
}
