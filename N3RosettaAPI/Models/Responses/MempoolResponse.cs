using Neo.IO.Json;
using System.Linq;

namespace Neo.Plugins
{
    public class MempoolResponse
    {
        public TransactionIdentifier[] TransactionIdentifiers { get; set; }

        public MempoolResponse(TransactionIdentifier[] transactionIdentifiers)
        {
            TransactionIdentifiers = transactionIdentifiers;
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["transaction_identifiers"] = TransactionIdentifiers.Select(p => p.ToJson()).ToArray();
            return json;
        }
    }
}
