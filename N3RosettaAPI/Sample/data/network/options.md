**Request**

```json
{
    "network_identifier": {
        "blockchain": "neo n3",
        "network": "testnet"
    }
}
```

**Response**

```json
{
    "version": {
        "rosetta_version": "1.4.10",
        "node_version": "/Neo:3.1.0/"
    },
    "allow": {
        "operation_statuses": [
            {
                "status": "SUCCESS",
                "successful": true
            },
            {
                "status": "FAILED",
                "successful": false
            }
        ],
        "operation_types": [
            "Transfer"
        ],
        "errors": [
            {
                "code": 1000,
                "message": "the network identifier is invalid",
                "retriable": false
            },
            {
                "code": 2000,
                "message": "the account identifier is invalid",
                "retriable": false
            },
            {
                "code": 2001,
                "message": "the address is invalid",
                "retriable": false
            },
            {
                "code": 2002,
                "message": "account not found",
                "retriable": false
            },
            {
                "code": 2003,
                "message": "the contract address is invalid",
                "retriable": false
            },
            {
                "code": 3000,
                "message": "the block identifier is invalid",
                "retriable": false
            },
            {
                "code": 3001,
                "message": "the block index is invalid",
                "retriable": false
            },
            {
                "code": 3002,
                "message": "the block hash is invalid",
                "retriable": false
            },
            {
                "code": 3003,
                "message": "block not found",
                "retriable": false
            },
            {
                "code": 4000,
                "message": "the transaction identifier is invalid",
                "retriable": false
            },
            {
                "code": 4001,
                "message": "the transaction hash is invalid",
                "retriable": false
            },
            {
                "code": 4002,
                "message": "transaction not found",
                "retriable": false
            },
            {
                "code": 5001,
                "message": "transaction deserialization failed",
                "retriable": false
            },
            {
                "code": 5002,
                "message": "the transaction is already signed",
                "retriable": false
            },
            {
                "code": 5003,
                "message": "no signature is passed in",
                "retriable": false
            },
            {
                "code": 5004,
                "message": "one or more signatures are invalid",
                "retriable": false
            },
            {
                "code": 5005,
                "message": "the curve type is not supported",
                "retriable": false
            },
            {
                "code": 5006,
                "message": "the public key is invalid",
                "retriable": false
            },
            {
                "code": 5007,
                "message": "the transaction witnesses are invalid",
                "retriable": false
            },
            {
                "code": 5008,
                "message": "the transaction metadata is invalid",
                "retriable": false
            },
            {
                "code": 5010,
                "message": "unknown error",
                "retriable": false
            },
            {
                "code": 5011,
                "message": "the transaction already exists and cannot be sent repeatedly",
                "retriable": false
            },
            {
                "code": 5012,
                "message": "the memory pool is full and no more transactions can be sent",
                "retriable": true
            },
            {
                "code": 5013,
                "message": "the transaction cannot be verified",
                "retriable": false
            },
            {
                "code": 5014,
                "message": "the transaction is invalid",
                "retriable": false
            },
            {
                "code": 5015,
                "message": "the transaction is expired",
                "retriable": false
            },
            {
                "code": 5016,
                "message": "the transaction is failed to verify due to insufficient fees",
                "retriable": false
            },
            {
                "code": 5017,
                "message": "one of the policy filters failed",
                "retriable": false
            },
            {
                "code": 6000,
                "message": "parse request body failed",
                "retriable": false
            },
            {
                "code": 6001,
                "message": "one or more params are invalid",
                "retriable": false
            },
            {
                "code": 6002,
                "message": "engine faulted",
                "retriable": false
            }
        ],
        "historical_balance_lookup": false,
        "timestamp_start_index": 1468595301000,
        "call_methods": [],
        "balance_exemption": [],
        "mempool_coins": false
    }
}
```
