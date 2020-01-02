using GraphQL.Types;
using Neo.IO.Json;
using System.Linq;

namespace GraphQLPlugin.ModelType
{
    public class PluginsType: ObjectGraphType<GraphPlugin>
    {
        public PluginsType()
        {
            Field(x => x.Name);
            Field(x => x.Version);
            Field(x => x.Interfaces, type: typeof(ListGraphType<StringGraphType>));
        }
    }
    public class GraphPlugin
    {
        public string Name { get; set; }

        public string Version { get; set; }

        public string[] Interfaces { get; set; }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["name"] = Name;
            json["version"] = Version;
            json["interfaces"] = new JArray(Interfaces.Select(p => (JObject)p));
            return json;
        }

        public static GraphPlugin FromJson(JObject json)
        {
            GraphPlugin plugin = new GraphPlugin();
            plugin.Name = json["name"].AsString();
            plugin.Version = json["version"].AsString();
            plugin.Interfaces = ((JArray)json["interfaces"]).Select(p => p.AsString()).ToArray();
            return plugin;
        }
    }
}
