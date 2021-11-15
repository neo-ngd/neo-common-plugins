using Neo.IO.Json;

namespace Neo.Plugins
{
    // Signature contains the payload that was signed, the public keys of the keypairs used to
    // produce the signature, the signature (encoded in hex), and the SignatureType. PublicKey is often
    // times not known during construction of the signing payloads but may be needed to combine
    // signatures properly.
    public class Signature
    {
        // SigningPayload is signed by the client with the keypair associated with an AccountIdentifier using the specified SignatureType.
        // SignatureType can be optionally populated if there is a restriction on the signature scheme that can be used to sign the payload.
        public SigningPayload SigningPayload { get; set; }
        // 	PublicKey contains a public key byte array for a particular CurveType encoded in hex.
        // 	Note that there is no PrivateKey struct as this is NEVER the concern of an implementation.
        public PublicKey PublicKey { get; set; }
        // 	SignatureType is the type of a cryptographic signature.
        public SignatureType SignatureType { get; set; }
        public string HexBytes { get; set; }

        public Signature(SigningPayload signingPayload, PublicKey publicKey, SignatureType signatureType, string hexBytes)
        {
            SigningPayload = signingPayload;
            PublicKey = publicKey;
            SignatureType = signatureType;
            HexBytes = hexBytes;
        }

        public static Signature FromJson(JObject json)
        {
            return new Signature(SigningPayload.FromJson(json["signing_payload"]),
                PublicKey.FromJson(json["public_key"]),
                json["signature_type"].ToSignatureType(),
                json["hex_bytes"].AsString());
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["signing_payload"] = SigningPayload.ToJson();
            json["public_key"] = PublicKey.ToJson();
            json["signature_type"] = SignatureType.AsString();
            json["hex_bytes"] = HexBytes;
            return json;
        }
    }
}
