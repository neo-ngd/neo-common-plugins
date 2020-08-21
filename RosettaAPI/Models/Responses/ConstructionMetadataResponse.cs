using Neo.IO.Json;
using System.Linq;

namespace Neo.Plugins
{
    public class ConstructionMetadataResponse
    {
        public Metadata Metadata { get; set; }
        public Amount[] SuggestedFee { get; set; }

        public ConstructionMetadataResponse(Metadata metadata, Amount[] suggestedFee = null)
        {
            Metadata = metadata;
            SuggestedFee = suggestedFee;
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["metadata"] = Metadata.ToJson();
            if (SuggestedFee != null && SuggestedFee.Length != 0)
                json["suggested_fee"] = SuggestedFee.Select(p => p.ToJson()).ToArray();
            return json;
        }
    }
}
