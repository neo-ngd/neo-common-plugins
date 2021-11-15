using Neo.IO.Json;
using System.Linq;

namespace Neo.Plugins
{
    // ConstructionPreprocessRequest is passed to the /construction/preprocess endpoint so that
    // a Rosetta implementation can determine which metadata it needs to request for construction.
    // Metadata provided in this object should NEVER be a product of live data (i.e. the caller must
    // follow some network-specific data fetching strategy outside of the Construction API to populate required Metadata).
    // If live data is required for construction, it MUST be fetched in the call to /construction/metadata.
    // The caller can provide a max fee they are willing to pay for a transaction.
    // This is an array in the case fees must be paid in multiple currencies.
    // The caller can also provide a suggested fee multiplier to indicate that the suggested fee should be scaled.
    // This may be used to set higher fees for urgent transactions or to pay lower fees when there is less urgency.
    // It is assumed that providing a very low multiplier (like 0.0001) will never lead to a transaction being created
    // with a fee less than the minimum network fee (if applicable).
    // In the case that the caller provides both a max fee and a suggested fee multiplier, the max fee will set
    // an upper bound on the suggested fee (regardless of the multiplier provided).
    public class ConstructionPreprocessRequest
    {
        public NetworkIdentifier NetworkIdentifier { get; set; }
        public Operation[] Operations { get; set; }
        public Metadata Metadata { get; set; }
        public Amount[] MaxFee { get; set; }
        public double? SuggestedFeeMultiplier { get; set; } // not useful in neo

        public ConstructionPreprocessRequest(NetworkIdentifier networkIdentifier, Operation[] operations, Metadata metadata = null,
            Amount[] maxFee = null, double? suggestedFeeMultiplier = null)
        {
            NetworkIdentifier = networkIdentifier;
            Operations = operations;
            Metadata = metadata;
            MaxFee = maxFee;
            SuggestedFeeMultiplier = suggestedFeeMultiplier;
        }

        public static ConstructionPreprocessRequest FromJson(JObject json)
        {
            return new ConstructionPreprocessRequest(NetworkIdentifier.FromJson(json["network_identifier"]),
                (json["operations"] as JArray).Select(p => Operation.FromJson(p)).ToArray(),
                json.ContainsProperty("metadata") ? Metadata.FromJson(json["metadata"]) : null,
                json.ContainsProperty("max_fee") ? (json["max_fee"] as JArray).ToList().Select(p => Amount.FromJson(p)).ToArray() : null,
                json.ContainsProperty("suggested_fee_multiplier") ? json["suggested_fee_multiplier"].AsNumber() : null);
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["network_identifier"] = NetworkIdentifier.ToJson();
            json["operations"] = Operations.Select(p => p.ToJson()).ToArray();
            if (Metadata != null && Metadata.ToJson() != null)
                json["metadata"] = Metadata.ToJson();
            if (SuggestedFeeMultiplier.HasValue)
                json["suggested_fee_multiplier"] = SuggestedFeeMultiplier.Value;
            return json;
        }
    }
}
