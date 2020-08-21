using Neo.IO.Json;
using System.Linq;

namespace Neo.Plugins
{
    public class NetworkStatusResponse
    {
        public BlockIdentifier CurrentBlockIdentifier { get; set; }
        public long CurrentBlockTimestamp { get; set; }
        public BlockIdentifier GenesisBlockIdentifier { get; set; }
        public BlockIdentifier OldestBlockIdentifier { get; set; }
        public Peer[] Peers { get; set; }

        public NetworkStatusResponse(BlockIdentifier  currentBlockIdentifier, long currentBlockTimestamp, BlockIdentifier genesisBlockIdentifier,
            Peer[] peers, BlockIdentifier oldestBlockIdentifier = null)
        {
            CurrentBlockIdentifier = currentBlockIdentifier;
            CurrentBlockTimestamp = currentBlockTimestamp;
            GenesisBlockIdentifier = genesisBlockIdentifier;
            Peers = peers;
            OldestBlockIdentifier = oldestBlockIdentifier;
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["current_block_identifier"] = CurrentBlockIdentifier.ToJson();
            json["current_block_timestamp"] = CurrentBlockTimestamp.ToString();
            json["genesis_block_identifier"] = GenesisBlockIdentifier.ToJson();
            if (OldestBlockIdentifier != null)
            {
                json["oldest_block_identifier"] = OldestBlockIdentifier.ToJson();
            }
            json["peers"] = Peers.Select(p => p.ToJson()).ToArray();
            return json;
        }
    }
}
