using Neo.IO.Json;
using System.Linq;

namespace Neo.Plugins
{
    // A BlockResponse includes a fully-populated block or a partially-populated block
    // with a list of other transactions to fetch (other_transactions).
    public class BlockResponse
    {
        public Block Block { get; set; }

        // Some blockchains may require additional transactions to be fetched that weren't returned in
        // the block response (ex: block only returns transaction hashes). For blockchains with a lot of
        // transactions in each block, this can be very useful as consumers can concurrently fetch all
        // transactions returned.
        public TransactionIdentifier[] OtherTransactions { get; set; }

        public BlockResponse(Block block, TransactionIdentifier[] otherTransactions = null)
        {
            Block = block;
            OtherTransactions = otherTransactions;
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["block"] = Block.ToJson();
            if (OtherTransactions != null)
                json["other_transactions"] = OtherTransactions.Select(p => p.ToJson()).ToArray();
            return json;
        }
    }
}
