using GraphQL.Types;
using Neo.IO.Json;

namespace GraphQLPlugin.ModelType
{
    public class VersionType: ObjectGraphType<GraphVersion>
    {
        public VersionType()
        {
            Field(x => x.TcpPort);
            Field(x => x.WsPort);
            Field(x => x.Nonce, type: typeof(UIntGraphType));
            Field(x => x.UserAgent);
        }
    }
    public class GraphVersion
    {
        public int TcpPort { get; set; }

        public int WsPort { get; set; }

        public uint Nonce { get; set; }

        public string UserAgent { get; set; }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["topPort"] = TcpPort.ToString();
            json["wsPort"] = WsPort.ToString();
            json["nonce"] = Nonce.ToString();
            json["useragent"] = UserAgent;
            return json;
        }

        public static GraphVersion FromJson(JObject json)
        {
            GraphVersion version = new GraphVersion();
            version.TcpPort = int.Parse(json["tcpPort"].AsString());
            version.WsPort = int.Parse(json["wsPort"].AsString());
            version.Nonce = uint.Parse(json["nonce"].AsString());
            version.UserAgent = json["useragent"].AsString();
            return version;
        }
    }

}
