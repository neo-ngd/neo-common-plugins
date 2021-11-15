using Neo.IO.Json;
using System.Linq;

namespace Neo.Plugins
{
    public class ConstructionParseResponse
    {
        public Operation[] Operations { get; set; }

        // All signers of a particular transaction. If the transaction is unsigned, it should be empty.
        public string[] Signers { get; set; }
        public Metadata Metadata { get; set; }

        public ConstructionParseResponse(Operation[] operations, string[] signers, Metadata metadata = null)
        {
            Operations = operations;
            Signers = signers;
            Metadata = metadata;
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["operations"] = Operations.Select(p => p.ToJson()).ToArray();
            json["signers"] = Signers.Select(p => new JString(p)).ToArray();
            if (Metadata != null && Metadata.ToJson() != null)
                json["metadata"] = Metadata.ToJson();
            return json;
        }
    }
}
