using Neo.IO.Json;

namespace Neo.Plugins
{
    public class SigningPayload
    {
        // The network-specific address of the account that should sign the payload.
        public string Address { get; set; }
        public byte[] Bytes { get; set; }
        public SignatureType SignatureType { get; set; }

        public SigningPayload(string address, byte[] bytes, SignatureType signatureType = SignatureType.Ecdsa)
        {
            Address = address;
            Bytes = bytes;
            SignatureType = signatureType;
        }

        public static SigningPayload FromJson(JObject json)
        {
            return new SigningPayload(json["address"].AsString(),
                json["hex_bytes"].AsString().HexToBytes(),
                json["signature_type"].ToSignatureType());
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["address"] = Address;
            json["hex_bytes"] = Bytes.ToHexString();
            json["signature_type"] = SignatureType.AsString();
            return json;
        }
    }
}
