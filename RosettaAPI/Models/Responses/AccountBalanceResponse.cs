using Neo.IO.Json;
using System.Linq;

namespace Neo.Plugins
{
    // An AccountBalanceResponse is returned on the /account/balance endpoint. If
    // an account has a balance for each AccountIdentifier describing it (ex: an NEP-5 token balance on
    // a few smart contracts), an account balance request must be made with each AccountIdentifier.
    public class AccountBalanceResponse
    {
        public BlockIdentifier BlockIdentifier { get; set; }

        // A single account may have a balance in multiple currencies.
        public Amount[] Balances { get; set; }

        // Account-based blockchains that utilize a nonce or sequence number should include that number
        // in the metadata. This number could be unique to the identifier or global across the account
        // address.
        public Metadata Metadata { get; set; }

        public AccountBalanceResponse(BlockIdentifier blockIdentifier, Amount[] balances, Metadata metadata = null)
        {
            BlockIdentifier = blockIdentifier;
            Balances = balances;
            Metadata = metadata;
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["block_identifier"] = BlockIdentifier.ToJson();
            json["balances"] = Balances.Select(p => p.ToJson()).ToArray();
            if (Metadata != null && Metadata.ToJson() != null)
                json["metadata"] = Metadata.ToJson();
            return json;
        }
    }
}
