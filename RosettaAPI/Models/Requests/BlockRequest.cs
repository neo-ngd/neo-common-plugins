using Neo.IO.Json;

namespace Neo.Plugins
{
    public class BlockRequest
    {
        public NetworkIdentifier NetworkIdentifier { get; set; }
        public PartialBlockIdentifier BlockIdentifier { get; set; }

        public BlockRequest(NetworkIdentifier networkIdentifier, PartialBlockIdentifier blockIdentifier)
        {
            NetworkIdentifier = networkIdentifier;
            BlockIdentifier = blockIdentifier;
        }

        public static BlockRequest FromJson(JObject json)
        {
            return new BlockRequest(NetworkIdentifier.FromJson(json["network_identifier"]),
                PartialBlockIdentifier.FromJson(json["block_identifier"]));
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["network_identifier"] = NetworkIdentifier.ToJson();
            json["block_identifier"] = BlockIdentifier.ToJson();
            return json;
        }
    }
}
