using Neo.IO.Json;

namespace Neo.Plugins
{
    // ConstructionCombineResponse ConstructionCombineResponse is returned by `/construction/combine`.
    // The network payload will be sent directly to the `construction/submit` endpoint.
    public class ConstructionCombineResponse
    {
        public string SignedTransaction { get; set; }

        public ConstructionCombineResponse(string signedTransaction)
        {
            SignedTransaction = signedTransaction;
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["signed_transaction"] = SignedTransaction;
            return json;
        }
    }
}
