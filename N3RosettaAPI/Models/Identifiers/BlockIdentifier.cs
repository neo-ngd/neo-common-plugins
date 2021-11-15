using Neo.IO.Json;

namespace Neo.Plugins
{
    public class BlockIdentifier
    {
        public long Index { get; set; }
        public string Hash { get; set; }

        public BlockIdentifier(long index, string hash)
        {
            Index = index;
            Hash = hash;
        }

        public static BlockIdentifier FromJson(JObject json)
        {
            return new BlockIdentifier((long)json["index"].AsNumber(), json["hash"].AsString());
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["index"] = Index.ToString();
            json["hash"] = Hash;
            return json;
        }
    }
}
