using GraphQL.Types;
using Neo.IO.Json;
using System.Linq;

namespace GraphQLPlugin.ModelType
{
    public class PeersType : ObjectGraphType<GraphPeers>
    {
        public PeersType()
        {
            Field(x => x.Unconnected, type: typeof(ListGraphType<PeerType>));
            Field(x => x.Bad, type: typeof(ListGraphType<PeerType>));
            Field(x => x.Connected, type: typeof(ListGraphType<PeerType>));
        }
    }

    public class PeerType: ObjectGraphType<GraphPeer>
    {
        public PeerType()
        {
            Field(x => x.Address);
            Field(x => x.Port);
        }
    }

    public class GraphPeers
    {
        public GraphPeer[] Unconnected { get; set; }

        public GraphPeer[] Bad { get; set; }

        public GraphPeer[] Connected { get; set; }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["unconnected"] = new JArray(Unconnected.Select(p => p.ToJson()));
            json["bad"] = new JArray(Bad.Select(p => p.ToJson()));
            json["connected"] = new JArray(Connected.Select(p => p.ToJson()));
            return json;
        }

        public static GraphPeers FromJson(JObject json)
        {
            GraphPeers result = new GraphPeers();
            result.Unconnected = ((JArray)json["unconnected"]).Select(p => GraphPeer.FromJson(p)).ToArray();
            result.Bad = ((JArray)json["bad"]).Select(p => GraphPeer.FromJson(p)).ToArray();
            result.Connected = ((JArray)json["connected"]).Select(p => GraphPeer.FromJson(p)).ToArray();
            return result;
        }
    }

    public class GraphPeer
    {
        public string Address { get; set; }

        public int Port { get; set; }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["address"] = Address;
            json["port"] = Port;
            return json;
        }

        public static GraphPeer FromJson(JObject json)
        {
            GraphPeer peer = new GraphPeer();
            peer.Address = json["address"].AsString();
            peer.Port = int.Parse(json["port"].AsString());
            return peer;
        }
    }
}
