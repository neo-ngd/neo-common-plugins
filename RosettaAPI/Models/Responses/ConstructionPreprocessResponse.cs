using Neo.IO.Json;

namespace Neo.Plugins
{
    public class ConstructionPreprocessResponse
    {
        public Metadata Options { get; set; }

        public ConstructionPreprocessResponse(Metadata options = null)
        {
            Options = options;
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            if (Options != null && Options.ToJson() != null)
                json["options"] = Options.ToJson();
            return json;
        }
    }
}
