using Neo.IO.Json;

namespace Neo.Plugins
{
    public class SigningPayload
    {
        // [DEPRECATED by account_identifier in v1.4.4]
        // The network-specific address of the account that should sign the payload.
        // public string Address { get; set; }

        // The account_identifier uniquely identifies an account within a network.
        // All fields in the account_identifier are utilized to determine this uniqueness(including the metadata field, if populated).
        public AccountIdentifier AccountIdentifier { get; set; }

        // signature bytes, required
        public string HexBytes { get; set; }
        // SignatureType is the type of a cryptographic signature.
        public SignatureType SignatureType { get; set; }

        public SigningPayload(string hexBytes, AccountIdentifier accountIdentifier = null, SignatureType signatureType = SignatureType.Ecdsa)
        {
            AccountIdentifier = accountIdentifier;
            HexBytes = hexBytes;
            SignatureType = signatureType;
        }

        public static SigningPayload FromJson(JObject json)
        {
            return new SigningPayload(json["hex_bytes"].AsString(),
                json.ContainsProperty("account_identifier") ? AccountIdentifier.FromJson(json["account_identifier"]) : null,
                json["signature_type"].ToSignatureType());
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            if (AccountIdentifier != null)
                json["account_identifier"] = AccountIdentifier.ToJson();
            json["hex_bytes"] = HexBytes;
            json["signature_type"] = SignatureType.AsString();
            return json;
        }
    }
}
