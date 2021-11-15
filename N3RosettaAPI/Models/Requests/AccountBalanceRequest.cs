using Neo.IO.Json;
using System.Linq;

namespace Neo.Plugins
{
    // An AccountBalanceRequest is utilized to make a balance request on the /account/balance endpoint.
    // If the block_identifier is populated, a historical balance query should be performed.
    public class AccountBalanceRequest
    {
        // The network_identifier specifies which network a particular object is associated with.
        public NetworkIdentifier NetworkIdentifier { get; set; }
        // The account_identifier uniquely identifies an account within a network.
        // All fields in the account_identifier are utilized to determine this uniqueness (including the metadata field, if populated).
        public AccountIdentifier AccountIdentifier { get; set; }
        // When fetching data by BlockIdentifier, it may be possible to only specify the index or hash.
        // If neither property is specified, it is assumed that the client is making a request at the current block.
        public PartialBlockIdentifier BlockIdentifier { get; set; }
        // In some cases, the caller may not want to retrieve all available balances for an AccountIdentifier.
        // If the currencies field is populated, only balances for the specified currencies will be returned.
        // If not populated, all available balances will be returned.
        public Currency[] Currencies { get; set; }

        public AccountBalanceRequest(NetworkIdentifier networkIdentifier, AccountIdentifier accountIdentifier, PartialBlockIdentifier blockIdentifier = null, Currency[] currencies = null)
        {
            NetworkIdentifier = networkIdentifier;
            AccountIdentifier = accountIdentifier;
            BlockIdentifier = blockIdentifier;
            Currencies = currencies;
        }

        public static AccountBalanceRequest FromJson(JObject json)
        {
            return new AccountBalanceRequest(NetworkIdentifier.FromJson(json["network_identifier"]),
                AccountIdentifier.FromJson(json["account_identifier"]),
                json.ContainsProperty("block_identifier") ? PartialBlockIdentifier.FromJson(json["block_identifier"]) : null,
                json.ContainsProperty("currencies") ? (json["currencies"] as JArray).Select(p => Currency.FromJson(p)).ToArray() : null);
        }
    }
}
