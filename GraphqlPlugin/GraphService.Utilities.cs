using Neo.IO.Json;
using Neo.Wallets;
using System.Linq;

namespace Neo.Plugins
{
    partial class GraphService: IGraphService
    {
        public JObject ListPlugins()
        {
            return new JArray(GraphQLServer.Plugins
                .OrderBy(u => u.Name)
                .Select(u => new JObject
                {
                    ["name"] = u.Name,
                    ["version"] = u.Version.ToString(),
                    ["interfaces"] = new JArray(u.GetType().GetInterfaces()
                        .Select(p => p.Name)
                        .Where(p => p.EndsWith("Plugin"))
                        .Select(p => (JObject)p))
                }));
        }

        public JObject ValidateAddress(string address)
        {
            JObject json = new JObject();
            UInt160 scriptHash;
            try
            {
                scriptHash = address.ToScriptHash();
            }
            catch
            {
                scriptHash = null;
            }
            json["address"] = address;
            json["isvalid"] = scriptHash != null;
            return json;
        }
    }
}
