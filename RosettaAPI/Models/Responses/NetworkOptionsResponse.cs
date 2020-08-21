using Neo.IO.Json;

namespace Neo.Plugins
{
    public class NetworkOptionsResponse
    {
        public Version Version { get; set; }
        public Allow Allow { get; set; }

        public NetworkOptionsResponse(Version version, Allow allow)
        {
            Version = version;
            Allow = allow;
        }
        
        public JObject ToJson()
        {
            JObject json = new JObject();
            json["version"] = Version.ToJson();
            json["allow"] = Allow.ToJson();
            return json;
        }
    }
}
