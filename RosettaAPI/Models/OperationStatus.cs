using Neo.IO.Json;

namespace Neo.Plugins
{
    // OperationStatus is utilized to indicate which Operation status are considered successful.
    public class OperationStatus
    {
        // The status is the network-specific status of the operation.
        public string Status { get; set; }

        // An Operation is considered successful if the Operation.Amount should affect the
        // Operation.Account. Some blockchains (like Bitcoin) only include successful operations in
        // blocks but other blockchains (like Ethereum) include unsuccessful operations that incur a
        // fee. To reconcile the computed balance from the stream of Operations, it is critical to
        // understand which Operation.Status indicate an Operation is successful and should affect an
        // Account.
        public bool Successful { get; set; }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["status"] = Status;
            json["successful"] = Successful.ToString().ToLower();
            return json;
        }

        public static readonly OperationStatus OPERATION_STATUS_SUCCESS = new OperationStatus() { Status = "SUCCESS", Successful = true };
        public static readonly OperationStatus OPERATION_STATUS_FAILED = new OperationStatus() { Status = "FAILED", Successful = false };
        public static readonly OperationStatus[] AllowedStatuses = new OperationStatus[] { OPERATION_STATUS_SUCCESS, OPERATION_STATUS_FAILED };
    }
}
