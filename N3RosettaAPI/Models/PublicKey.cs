using Neo.IO.Json;

namespace Neo.Plugins
{
    /// <summary>
    /// PublicKey contains a public key byte array for a particular CurveType encoded in hex. 
    /// Note that there is no PrivateKey struct as this is NEVER the concern of an implementation.
    /// </summary>
    public class PublicKey
    {
        // 	Hex-encoded public key bytes in the format specified by the CurveType.
        public string HexBytes { get; set; } // bytes in hex string, eg.: "0390de1b591204d4423819660cfa51d18b371b9991e551971b59bd20c40b245078"
        // CurveType is the type of cryptographic curve associated with a PublicKey.
        public CurveType CurveType { get; set; }
        /// <summary>
        /// address hash
        /// </summary>
        public UInt160 AddressHash { get; set; }

        public PublicKey(string hexBytes, CurveType curveType)
        {
            HexBytes = hexBytes;
            CurveType = curveType;
            AddressHash = hexBytes.ToUInt160FromPublicKey();
        }

        public static PublicKey FromJson(JObject json)
        {
            return new PublicKey(json["hex_bytes"].AsString(),
                json["curve_type"].ToCurveType());
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["hex_bytes"] = HexBytes;
            json["curve_type"] = CurveType.AsString();
            return json;
        }
    }
}
