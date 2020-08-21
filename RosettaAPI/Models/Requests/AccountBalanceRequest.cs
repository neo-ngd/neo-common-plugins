using Neo.IO.Json;

namespace Neo.Plugins
{
    public class AccountBalanceRequest
    {
        public NetworkIdentifier NetworkIdentifier { get; set; }
        public AccountIdentifier AccountIdentifier { get; set; }
        public PartialBlockIdentifier BlockIdentifier { get; set; }

        public AccountBalanceRequest(NetworkIdentifier networkIdentifier, AccountIdentifier accountIdentifier, PartialBlockIdentifier blockIdentifier = null)
        {
            NetworkIdentifier = networkIdentifier;
            AccountIdentifier = accountIdentifier;
            BlockIdentifier = blockIdentifier;
        }

        public static AccountBalanceRequest FromJson(JObject json)
        {
            return new AccountBalanceRequest(NetworkIdentifier.FromJson(json["network_identifier"]),
                AccountIdentifier.FromJson(json["account_identifier"]),
                json.ContainsProperty("block_identifier") ? PartialBlockIdentifier.FromJson(json["block_identifier"]) : null);
        }
    }
}
