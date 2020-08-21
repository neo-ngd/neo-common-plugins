using Neo.IO.Json;
using System.Linq;

namespace Neo.Plugins
{
    public class ConstructionCombineRequest
    {
        public NetworkIdentifier NetworkIdentifier { get; set; }
        public string UnsignedTransaction { get; set; }
        public Signature[] Signatures { get; set; }

        public ConstructionCombineRequest(NetworkIdentifier networkIdentifier, string unsignedTransaction, Signature[] signatures)
        {
            NetworkIdentifier = networkIdentifier;
            UnsignedTransaction = unsignedTransaction;
            Signatures = signatures;
        }

        public static ConstructionCombineRequest FromJson(JObject json)
        {
            return new ConstructionCombineRequest(NetworkIdentifier.FromJson(json["network_identifier"]),
                json["unsigned_transaction"].AsString(),
                (json["signatures"] as JArray).Select(p => Signature.FromJson(p)).ToArray());
        }
    }
}
