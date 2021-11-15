using Neo.IO.Json;
using System.Linq;

namespace Neo.Plugins
{
    // ConstructionPreprocessResponse contains options that will be sent unmodified to /construction/metadata.
    // If it is not necessary to make a request to /construction/metadata, options should be omitted.
    // Some blockchains require the PublicKey of particular AccountIdentifiers to construct a valid transaction.
    // To fetch these PublicKeys, populate required_public_keys with the AccountIdentifiers associated with the desired PublicKeys.
    // If it is not necessary to retrieve any PublicKeys for construction, required_public_keys should be omitted.
    public class ConstructionPreprocessResponse
    {
        // The options that will be sent directly to /construction/metadata by the caller.
        public Metadata Options { get; set; }

        public AccountIdentifier[] RequiredPublicKeys { get; set; }

        public ConstructionPreprocessResponse(Metadata options = null, AccountIdentifier[] requiredPublicKeys = null)
        {
            Options = options;
            RequiredPublicKeys = requiredPublicKeys;
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            if (Options != null && Options.ToJson() != null)
                json["options"] = Options.ToJson();
            if (RequiredPublicKeys != null)
                json["required_public_keys"] = RequiredPublicKeys.ToList().Select(p => p.ToJson()).ToArray();
            return json;
        }
    }
}
