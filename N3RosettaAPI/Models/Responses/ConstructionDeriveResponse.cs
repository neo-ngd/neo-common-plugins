using Neo.IO.Json;

namespace Neo.Plugins
{
    public class ConstructionDeriveResponse
    {
        public AccountIdentifier AccountIdentifier { get; set; }
        public Metadata Metadata { get; set; }

        public ConstructionDeriveResponse(AccountIdentifier accountIdentifier = null, Metadata metadata = null)
        {
            AccountIdentifier = accountIdentifier;
            Metadata = metadata;
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            if (AccountIdentifier != null && AccountIdentifier.ToJson() != null)
                json["account_identifier"] = AccountIdentifier.ToJson();
            if (Metadata != null && Metadata.ToJson() != null)
                json["metadata"] = Metadata.ToJson();
            return json;
        }
    }
}
