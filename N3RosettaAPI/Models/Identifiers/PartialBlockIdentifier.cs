using Neo.IO.Json;

namespace Neo.Plugins
{
    // The PartialBlockIdentifier class is used when fetching data by BlockIdentifier, it may be possible to only specify
    // the index or hash. If neither property is specified, it is assumed that the client is making a
    // request at the current block.
    public class PartialBlockIdentifier
    {
        public long? Index { get; set; }
        public string Hash { get; set; }

        public PartialBlockIdentifier(long? index = null, string hash = null)
        {
            Index = index;
            Hash = hash;
        }

        public static PartialBlockIdentifier FromJson(JObject json)
        {
            if (json is null) return null;
            return new PartialBlockIdentifier(json.ContainsProperty("index") ? (long?)json["index"].AsNumber() : null,
                json.ContainsProperty("hash") ? json["hash"].AsString() : null);
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            if (Index != null && Index >= 0)
                json["index"] = Index;
            if (!string.IsNullOrEmpty(Hash))
                json["hash"] = Hash;
            return json;
        }
    }
}
