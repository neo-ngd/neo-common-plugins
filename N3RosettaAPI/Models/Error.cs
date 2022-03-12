using Neo.IO.Json;

namespace Neo.Plugins
{
    // Instead of utilizing HTTP status codes to describe node errors (which often do not have a good analog),
    // rich errors are returned using this object.
    // Both the code and message fields can be individually used to correctly identify an error.
    // Implementations MUST use unique values for both fields.
    public class Error
    {
        // Code is a network-specific error code. If desired, this code can be equivalent to an HTTP
        // status code.
        public int Code { get; set; }
        // Message is a network-specific error message. The message MUST NOT change for a given code.
        // In particular, this means that any contextual information should be included in the details field.
        public string Message { get; set; }
        // Description allows the implementer to optionally provide additional information about an error.
        // In many cases, the content of this field will be a copy-and-paste from existing developer documentation.
        // Description can ONLY be populated with generic information about a particular type of error.
        // It MUST NOT be populated with information about a particular instantiation of an error (use details for this).
        // Whereas the content of Error.Message should stay stable across releases,
        // the content of Error.Description will likely change across releases (as implementers improve error documentation).
        // For this reason, the content in this field is not part of any type assertion (unlike Error.Message).
        public string Description { get; set; }
        // An error is retriable if the same request may succeed if submitted again.
        public bool Retriable { get; set; }
        // Often times it is useful to return context specific to the request that caused the error
        // (i.e. a sample of the stack trace or impacted account) in addition to the standard error message.
        public Metadata Details { get; set; }

        public Error(int code, string message, string description = null, bool retriable = false, Metadata details = null)
        {
            Code = code;
            Message = message;
            Description = description;
            Retriable = retriable;
            Details = details;
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["code"] = Code;
            json["message"] = Message;
            if (Description != null)
                json["description"] = Description;
            json["retriable"] = Retriable;
            if (Details != null && Details.ToJson() != null)
                json["details"] = Details.ToJson();
            return json;
        }

        #region Network
        public static readonly Error NETWORK_IDENTIFIER_INVALID = new Error(1000, "the network identifier is invalid");
        #endregion

        #region Account
        public static readonly Error ACCOUNT_IDENTIFIER_INVALID = new Error(2000, "the account identifier is invalid");
        public static readonly Error ACCOUNT_ADDRESS_INVALID = new Error(2001, "the address is invalid");
        public static readonly Error ACCOUNT_NOT_FOUND = new Error(2002, "account not found");
        public static readonly Error CONTRACT_ADDRESS_INVALID = new Error(2003, "the contract address is invalid");
        #endregion

        #region Block
        public static readonly Error BLOCK_IDENTIFIER_INVALID = new Error(3000, "the block identifier is invalid");
        public static readonly Error BLOCK_INDEX_INVALID = new Error(3001, "the block index is invalid");
        public static readonly Error BLOCK_HASH_INVALID = new Error(3002, "the block hash is invalid");
        public static readonly Error BLOCK_NOT_FOUND = new Error(3003, "block not found");
        #endregion

        #region Mempool
        public static readonly Error TX_IDENTIFIER_INVALID = new Error(4000, "the transaction identifier is invalid");
        public static readonly Error TX_HASH_INVALID = new Error(4001, "the transaction hash is invalid");
        public static readonly Error TX_NOT_FOUND = new Error(4002, "transaction not found");
        #endregion

        #region Construction
        public static readonly Error TX_DESERIALIZE_ERROR = new Error(5001, "transaction deserialization failed");
        public static readonly Error TX_ALREADY_SIGNED = new Error(5002, "the transaction is already signed");
        public static readonly Error NO_SIGNATURE = new Error(5003, "no signature is passed in");
        public static readonly Error INVALID_SIGNATURES = new Error(5004, "one or more signatures are invalid");
        public static readonly Error CURVE_NOT_SUPPORTED = new Error(5005, "the curve type is not supported");
        public static readonly Error INVALID_PUBLIC_KEY = new Error(5006, "the public key is invalid");
        public static readonly Error TX_WITNESSES_INVALID = new Error(5007, "the transaction witnesses are invalid");
        public static readonly Error TX_METADATA_INVALID = new Error(5008, "the transaction metadata is invalid");

        public static readonly Error UNKNOWN_ERROR = new Error(5010, "unknown error");
        public static readonly Error ALREADY_EXISTS = new Error(5011, "the transaction already exists and cannot be sent repeatedly");
        public static readonly Error OUT_OF_MEMORY = new Error(5012, "the memory pool is full and no more transactions can be sent", retriable: true);
        public static readonly Error UNABLE_TO_VERIFY = new Error(5013, "the transaction cannot be verified");
        public static readonly Error TX_INVALID = new Error(5014, "the transaction is invalid");
        public static readonly Error TX_EXPIRED = new Error(5015, "the transaction is expired");
        public static readonly Error INSUFFICIENT_FUNDS = new Error(5016, "the transaction is failed to verify due to insufficient fees");
        public static readonly Error POLICY_FAIL = new Error(5017, "one of the policy filters failed");
        #endregion

        #region Other
        public static readonly Error PARSE_REQUEST_ERROR = new Error(6000, "parse request body failed");
        public static readonly Error PARAMETER_INVALID = new Error(6001, "one or more params are invalid");
        public static readonly Error VM_FAULT = new Error(6002, "engine faulted");
        #endregion

        public static readonly Error[] AllowedErrors = new Error[]
        {
            NETWORK_IDENTIFIER_INVALID,

            ACCOUNT_IDENTIFIER_INVALID,
            ACCOUNT_ADDRESS_INVALID,
            ACCOUNT_NOT_FOUND,
            CONTRACT_ADDRESS_INVALID,

            BLOCK_IDENTIFIER_INVALID,
            BLOCK_INDEX_INVALID,
            BLOCK_HASH_INVALID,
            BLOCK_NOT_FOUND,

            TX_IDENTIFIER_INVALID,
            TX_HASH_INVALID,
            TX_NOT_FOUND,

            TX_DESERIALIZE_ERROR,
            TX_ALREADY_SIGNED,
            NO_SIGNATURE,
            INVALID_SIGNATURES,
            CURVE_NOT_SUPPORTED,
            INVALID_PUBLIC_KEY,
            TX_WITNESSES_INVALID,
            TX_METADATA_INVALID,

            UNKNOWN_ERROR,
            ALREADY_EXISTS,
            OUT_OF_MEMORY,
            UNABLE_TO_VERIFY,
            TX_INVALID,
            TX_EXPIRED,
            INSUFFICIENT_FUNDS,
            POLICY_FAIL,

            PARSE_REQUEST_ERROR,
            PARAMETER_INVALID,
            VM_FAULT
        };
    }
}
