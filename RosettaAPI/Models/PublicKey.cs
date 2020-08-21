using Neo.IO.Json;

namespace Neo.Plugins
{
    public class PublicKey
    {
        public byte[] Bytes { get; set; }
        public CurveType CurveType { get; set; }

        public PublicKey(byte[] bytes, CurveType curveType)
        {
            Bytes = bytes;
            CurveType = curveType;
        }

        public static PublicKey FromJson(JObject json)
        {
            return new PublicKey(json["hex_bytes"].AsString().HexToBytes(),
                json["curve_type"].ToCurveType());
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["hex_bytes"] = Bytes.ToHexString();
            json["curve_type"] = CurveType.AsString();
            return json;
        }
    }
}
