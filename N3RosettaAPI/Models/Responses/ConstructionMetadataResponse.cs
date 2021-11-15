using Neo.IO.Json;
using System.Linq;

namespace Neo.Plugins
{
    // The ConstructionMetadataResponse returns network-specific metadata used for transaction construction.
    // Optionally, the implementer can return the suggested fee associated with the transaction being constructed.
    // The caller may use this info to adjust the intent of the transaction or to create a transaction with a different account that can pay the suggested fee.
    // Suggested fee is an array in case fee payment must occur in multiple currencies.
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
