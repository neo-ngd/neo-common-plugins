using Neo.IO.Json;

namespace Neo.Plugins
{
    public class Peer
    {
        public string PeerID { get; set; }
        public Metadata Metadata { get; set; }

        public Peer(string peerID, Metadata metadata = null)
        {
            PeerID = peerID;
            Metadata = metadata;
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["peer_id"] = PeerID;
            if (Metadata != null && Metadata.ToJson() != null)
                json["metadata"] = Metadata.ToJson();
            return json;
        }
    }
}
