using Neo.IO.Json;

namespace Neo.Plugins
{
    // Error Instead of utilizing HTTP status codes to describe node errors (which often do not have a
    // good analog), rich errors are returned using this object.
    public class Error
    {
        // Code is a network-specific error code. If desired, this code can be equivalent to an HTTP
        // status code.
        public int Code { get; set; }
        public string Message { get; set; }
        public bool Retriable { get; set; }
        public Metadata Details { get; set; }

        public Error(int code, string message, bool retriable = false, Metadata details = null)
        {
            Code = code;
            Message = message;
            Retriable = retriable;
            Details = details;
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["code"] = Code.ToString();
            json["message"] = Message;
            json["retriable"] = Retriable.ToString().ToLower();
            if (Details != null && Details.ToJson() != null)
                json["details"] = Details.ToJson();
            return json;
        }

        #region Network
        public static readonly Error NETWORK_IDENTIFIER_INVALID = new Error(1000, "network identifier is invalid", false);
        #endregion

        #region Account
        public static readonly Error ACCOUNT_IDENTIFIER_INVALID = new Error(2000, "account identifier is invalid", false);
        public static readonly Error ACCOUNT_ADDRESS_INVALID = new Error(2001, "address is invalid", false);
        public static readonly Error ACCOUNT_NOT_FOUND = new Error(2002, "account not found", false);
        public static readonly Error CONTRACT_ADDRESS_INVALID = new Error(2003, "contract address is invalid", false);
        #endregion

        #region Block
        public static readonly Error BLOCK_IDENTIFIER_INVALID = new Error(3000, "block identifier is invalid", false);
        public static readonly Error BLOCK_INDEX_INVALID = new Error(3001, "block index is invalid", false);
        public static readonly Error BLOCK_HASH_INVALID = new Error(3002, "block hash is invalid", false);
        public static readonly Error BLOCK_NOT_FOUND = new Error(3003, "block not found", false);
        #endregion

        #region Mempool
        public static readonly Error TX_IDENTIFIER_INVALID = new Error(4000, "transaction identifier is invalid", false);
        public static readonly Error TX_HASH_INVALID = new Error(4001, "transaction hash is invalid", false);
        public static readonly Error TX_NOT_FOUND = new Error(4002, "transaction not found", false);
        #endregion

        #region Construction
        public static readonly Error TX_DESERIALIZE_ERROR = new Error(5001, "transaction deserialization error");
        public static readonly Error TX_ALREADY_SIGNED = new Error(5002, "transaction is already signed");
        public static readonly Error NO_SIGNATURE = new Error(5003, "no signature is passed in");
        public static readonly Error INVALID_SIGNATURES = new Error(5004, "one or more signatures are invalid");
        public static readonly Error CURVE_NOT_SUPPORTED = new Error(5005, "curve type is not supported");
        public static readonly Error INVALID_PUBLIC_KEY = new Error(5006, "public key is invalid");
        public static readonly Error TX_WITNESS_INVALID = new Error(5007, "transaction witness is invalid");
        public static readonly Error TX_METADATA_INVALID = new Error(5008, "transaction metadata is invalid");

        public static readonly Error UNKNOWN_ERROR = new Error(5010, "Unknown error.");
        public static readonly Error ALREADY_EXISTS = new Error(5011, "The transaction already exists and cannot be sent repeatedly.");
        public static readonly Error OUT_OF_MEMORY = new Error(5012, "The memory pool is full and no more transactions can be sent.", true);
        public static readonly Error UNABLE_TO_VERIFY = new Error(5013, "The transaction cannot be verified.");
        public static readonly Error TX_INVALID = new Error(5014, "The transaction is invalid.");
        public static readonly Error POLICY_FAIL = new Error(5015, "One of the policy filters failed.");
        #endregion

        #region Other
        public static readonly Error PARSE_REQUEST_ERROR = new Error(6000, "parse request body error");
        public static readonly Error PARAMETER_INVALID = new Error(6001, "one or more params are invalid", false);
        public static readonly Error VM_FAULT = new Error(6002, "engine faulted", false);
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
            TX_WITNESS_INVALID,
            TX_METADATA_INVALID,

            UNKNOWN_ERROR,
            ALREADY_EXISTS,
            OUT_OF_MEMORY,
            UNABLE_TO_VERIFY,
            TX_INVALID,
            POLICY_FAIL,

            PARSE_REQUEST_ERROR,
            PARAMETER_INVALID,
            VM_FAULT
        };
    }
}
