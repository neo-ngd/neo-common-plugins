using Neo.IO.Json;
using System.Linq;

namespace Neo.Plugins
{
    // Block contains an array of Transactions that occurred at a particular BlockIdentifier.
    public class Block
    {
        public BlockIdentifier BlockIdentifier { get; set; }
        public BlockIdentifier ParentBlockIdentifier { get; set; }
        // The timestamp of the block in milliseconds since the Unix Epoch. The timestamp is stored in
        // milliseconds because some blockchains produce blocks more often than once a second.
        public long Timestamp { get; set; }
        public Transaction[] Transactions { get; set; }
        public Metadata Metadata { get; set; }

        public Block(BlockIdentifier blockIdentifier, BlockIdentifier parentBlockIdentifier,
            long timestamp, Transaction[] transactions, Metadata metadata = null)
        {
            BlockIdentifier = blockIdentifier;
            ParentBlockIdentifier = parentBlockIdentifier;
            Timestamp = timestamp;
            Transactions = transactions;
            Metadata = metadata;
        }

        public static Block FromJson(JObject json)
        {
            return new(
                BlockIdentifier.FromJson(json["block_identifier"]),
                BlockIdentifier.FromJson(json["parent_block_identifier"]),
                (long)json["timestamp"].GetNumber(),
                json["transactions"].GetArray().Select(p => Transaction.FromJson(p)).ToArray(),
                Metadata.FromJson(json["metadata"])
            );
        }

        public JObject ToJson()
        {
            JObject json = new();
            json["block_identifier"] = BlockIdentifier.ToJson();
            json["parent_block_identifier"] = ParentBlockIdentifier.ToJson();
            json["timestamp"] = Timestamp;
            json["transactions"] = Transactions.Select(p => p.ToJson()).ToArray();
            if (Metadata != null && Metadata.ToJson() != null)
                json["metadata"] = Metadata.ToJson();
            return json;
        }
    }
}
