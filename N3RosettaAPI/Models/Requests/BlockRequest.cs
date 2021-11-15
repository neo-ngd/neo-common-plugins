using Neo.IO.Json;

namespace Neo.Plugins
{
    // A BlockRequest is utilized to make a block request on the /block endpoint.
    public class BlockRequest
    {
        // The network_identifier specifies which network a particular object is associated with.
        public NetworkIdentifier NetworkIdentifier { get; set; }

        // When fetching data by BlockIdentifier, it may be possible to only specify the index or hash.
        // If neither property is specified, it is assumed that the client is making a request at the current block.
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
