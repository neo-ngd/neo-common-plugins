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

        // If populated, timestamp_start_index indicates the first block index where block timestamps are
        // considered valid (i.e. all blocks less than timestamp_start_index could have invalid timestamps).
        // This is useful when the genesis block (or blocks) of a network have timestamp 0.
        // If not populated, block timestamps are assumed to be valid for all available blocks.
        public long TimestampStartIndex { get; set; }

        // All methods that are supported by the /call endpoint. 
        // Communicating which parameters should be provided to /call is the responsibility of the implementer 
        // (this is en lieu of defining an entire type system and requiring the implementer to define that in Allow).
        public string[] CallMethods { get; set; }

        // BalanceExemptions is an array of BalanceExemption indicating which account balances could change without a corresponding Operation.
        public BalanceExemption[] BalanceExemptions { get; set; }

        // Any Rosetta implementation that can update an AccountIdentifier's unspent coins based on the contents of the mempool should populate this field as true.
        // If false, requests to /account/coins that set include_mempool as true will be automatically rejected.
        public bool MempoolCoins { get; set; }

        public Allow(OperationStatus[] operationStatuses, string[] operationTypes, Error[] errors, bool historicalBalanceLookup,
            string[] callMethods, BalanceExemption[] balanceExemptions, bool mempoolCoins, long timestampStartIndex = -1)
        {
            OperationStatuses = operationStatuses;
            OperationTypes = operationTypes;
            Errors = errors;
            HistoricalBalanceLookup = historicalBalanceLookup;
            TimestampStartIndex = timestampStartIndex;
            CallMethods = callMethods;
            BalanceExemptions = balanceExemptions;
            MempoolCoins = mempoolCoins;
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["operation_statuses"] = OperationStatuses.Select(p => p.ToJson()).ToArray();
            json["operation_types"] = OperationTypes.Select(p => new JString(p)).ToArray();
            json["errors"] = Errors.Select(p => p.ToJson()).ToArray();
            json["historical_balance_lookup"] = new JBoolean(HistoricalBalanceLookup);
            if (TimestampStartIndex >= 0)
                json["timestamp_start_index"] = TimestampStartIndex;
            json["call_methods"] = CallMethods.Select(p => new JString(p)).ToArray();
            json["balance_exemption"] = BalanceExemptions.Select(p => p.ToJson()).ToArray();
            json["mempool_coins"] = new JBoolean( MempoolCoins);
            return json;
        }
    }
}
