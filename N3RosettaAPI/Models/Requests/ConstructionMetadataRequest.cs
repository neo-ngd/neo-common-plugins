using Neo.IO.Json;
using System.Linq;

namespace Neo.Plugins
{
    // A ConstructionMetadataRequest is utilized to get information required to construct a transaction.
    // The Options object used to specify which metadata to return is left purposely unstructured to allow flexibility for implementers.
    // Options is not required in the case that there is network-wide metadata of interest.
    // Optionally, the request can also include an array of PublicKeys associated with the AccountIdentifiers returned in ConstructionPreprocessResponse.
    public class ConstructionMetadataRequest
    {
        public NetworkIdentifier NetworkIdentifier { get; set; }

        // Some blockchains require different metadata for different types of transaction construction (ex: delegation versus a transfer).
        // Instead of requiring a blockchain node to return all possible types of metadata for construction
        // (which may require multiple node fetches), the client can populate an options object to limit the metadata returned to only the subset required.
        public Metadata Options { get; set; }

        public PublicKey[] PublicKeys { get; set; } 

        public ConstructionMetadataRequest(NetworkIdentifier networkIdentifier, Metadata options = null, PublicKey[] publicKeys = null)
        {
            NetworkIdentifier = networkIdentifier;
            Options = options;
            PublicKeys = publicKeys;
        }

        public static ConstructionMetadataRequest FromJson(JObject json)
        {
            return new ConstructionMetadataRequest(NetworkIdentifier.FromJson(json["network_identifier"]),
                json.ContainsProperty("options") ? Metadata.FromJson(json["options"]) : null,
                json.ContainsProperty("public_keys") ? (json["public_keys"] as JArray).Select(p => PublicKey.FromJson(p)).ToArray() : null);
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["network_identifier"] = NetworkIdentifier.ToJson();
            if (Options != null)
                json["options"] = Options.ToJson();
            return json;
        }
    }
}
