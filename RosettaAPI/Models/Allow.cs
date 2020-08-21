using Neo.IO.Json;
using System.Linq;

namespace Neo.Plugins
{
    // The Allow class specifies supported Operation status, Operation types, and all possible error
    // statuses. This Allow object is used by clients to validate the correctness of a Rosetta Server
    // implementation. It is expected that these clients will error if they receive some response that
    // contains any of the above information that is not specified here.
    public class Allow
    {
        // All Operation.Status this implementation supports. Any status that is returned during parsing
        // that is not listed here will cause client validation to error.
        public OperationStatus[] OperationStatuses { get; set; }

        // All Operation.Type this implementation supports. Any type that is returned during parsing
        // that is not listed here will cause client validation to error.
        public string[] OperationTypes { get; set; }

        // All Errors that this implementation could return. Any error that is returned during parsing
        // that is not listed here will cause client validation to error.
        public Error[] Errors { get; set; }

        // Any Rosetta implementation that supports querying the balance of an account at any height in
        // the past should set this to true.
        public bool HistoricalBalanceLookup { get; set; }

        public Allow(OperationStatus[] operationStatuses, string[] operationTypes, Error[] errors, bool historicalBalanceLookup)
        {
            OperationStatuses = operationStatuses;
            OperationTypes = operationTypes;
            Errors = errors;
            HistoricalBalanceLookup = historicalBalanceLookup;
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["operation_statuses"] = OperationStatuses.Select(p => p.ToJson()).ToArray();
            json["operation_types"] = OperationTypes.Select(p => new JString(p)).ToArray();
            json["errors"] = Errors.Select(p => p.ToJson()).ToArray();
            json["historical_balance_lookup"] = HistoricalBalanceLookup.ToString().ToLower();
            return json;
        }
    }
}
