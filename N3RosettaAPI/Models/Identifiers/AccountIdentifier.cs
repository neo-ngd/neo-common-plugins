using Neo.IO.Json;

namespace Neo.Plugins
{
    public class AccountIdentifier
    {
        public string Address { get; set; }
        public SubAccountIdentifier SubAccountIdentifier { get; set; }
        public Metadata Metadata { get; set; }

        public AccountIdentifier(string address, SubAccountIdentifier subAccountIdentifier = null, Metadata metadata = null)
        {
            Address = address;
            SubAccountIdentifier = subAccountIdentifier;
            Metadata = metadata;
        }

        public static AccountIdentifier FromJson(JObject json)
        {
            if (json is null) return null;
            return new AccountIdentifier(json["address"].AsString(),
                json.ContainsProperty("sub_account") ? SubAccountIdentifier.FromJson(json["sub_account"]) : null,
                json.ContainsProperty("metadata") ? Metadata.FromJson(json["metadata"]) : null);
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["address"] = Address;
            if (SubAccountIdentifier != null)
                json["sub_account"] = SubAccountIdentifier.ToJson();
            if (Metadata != null && Metadata.ToJson() != null)
                json["metadata"] = Metadata.ToJson();
            return json;
        }
    }
}
