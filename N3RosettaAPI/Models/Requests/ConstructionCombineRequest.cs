using Neo.IO.Json;
using System.Linq;

namespace Neo.Plugins
{
    // ConstructionCombineRequest is the input to the /construction/combine endpoint.
    // It contains the unsigned transaction blob returned by /construction/payloads and all required signatures to create a network transaction.
    public class ConstructionCombineRequest
    {
        // The network_identifier specifies which network a particular object is associated with.
        public NetworkIdentifier NetworkIdentifier { get; set; }
        // unsigned tx in base64 string
        public string UnsignedTransaction { get; set; }
        // signatures
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
                (json["signatures"] as JArray).Select(p => Signature.FromJson(p)).ToArray()
                );
        }
    }
}
