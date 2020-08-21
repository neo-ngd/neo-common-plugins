using Neo.IO.Json;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Plugins
{
    public class Metadata
    {
        public Dictionary<string, JObject> Pairs { get; set; }

        public JObject this[string s]
        {
            get { return Pairs[s]; }
            set { Pairs[s] = value; }
        }

        public Metadata(Dictionary<string, JObject> pairs)
        {
            Pairs = pairs;
        }

        public bool ContainsKey(string key)
        {
            return Pairs.ContainsKey(key);
        }

        public bool TryGetValue(string key, out JObject value)
        {
            return Pairs.TryGetValue(key, out value);
        }

        public static Metadata FromJson(JObject json)
        {
            if (json is null || json.Properties is null)
                return null;
            return new Metadata(json.Properties.ToDictionary(p => p.Key, p => p.Value));
        }

        public JObject ToJson()
        {
            if (Pairs != null && Pairs.Count > 0)
            {
                JObject meta = new JObject();
                foreach (var item in Pairs)
                    meta[item.Key] = item.Value;
                return meta;
            }
            return null;
        }
    }
}
