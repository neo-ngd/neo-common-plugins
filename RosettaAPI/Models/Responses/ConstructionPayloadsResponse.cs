using Neo.IO.Json;
using System.Linq;

namespace Neo.Plugins
{
    // ConstructionTransactionResponse is returned by
    // `/construction/payloads`. It contains an unsigned transaction blob (that is usually needed to
    // construct the a network transaction from a collection of signatures) and an array of payloads
    // that must be signed by the caller.
    public class ConstructionPayloadsResponse
    {
        public string UnsignedTransaction { get; set; }
        public SigningPayload[] Payloads { get; set; }

        public ConstructionPayloadsResponse(string unsignedTransaction, SigningPayload[] payloads)
        {
            UnsignedTransaction = unsignedTransaction;
            Payloads = payloads;
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["unsigned_transaction"] = UnsignedTransaction;
            json["payloads"] = Payloads.Select(p => p.ToJson()).ToArray();
            return json;
        }
    }
}
