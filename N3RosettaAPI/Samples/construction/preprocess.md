**Request**

```json
{
    "network_identifier": {
        "blockchain": "neo n3",
        "network": "testnet"
    },
    "operations": [
        {
            "operation_identifier": {
                "index": 0
            },
            "type": "Transfer",
            "account": {
                "address": "NgGCEfyNoSaRZYxoj6WdJqkZnov3DAaY6F"
            },
            "amount": {
                "value": "-1",
                "currency": {
                    "symbol": "NEO",
                    "decimals": 0,
                    "metadata": {
                        "script_hash": "0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5"
                    }
                }
            }
        },
        {
            "operation_identifier": {
                "index": 1
            },
            "related_operations": [
                {
                    "index": 0
                }
            ],
            "type": "Transfer",
            "account": {
                "address": "NPS3U9PduobRCai5ZUdK2P3Y8RjwzMVfSg"
            },
            "amount": {
                "value": "1",
                "currency": {
                    "symbol": "NEO",
                    "decimals": 0,
                    "metadata": {
                        "script_hash": "0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5"
                    }
                }
            }
        }
    ],
    "metadata": {
        "signer_metadata": [
            {
                "m": 0,
                "related_accounts": [],
                "signer_account": "NcpevbKeAFioFRs8zdB5J9UkMZQxKBwExw"
            },
            {
                "m": 0,
                "related_accounts": [],
                "signer_account": "NgGCEfyNoSaRZYxoj6WdJqkZnov3DAaY6F"
            }
        ],
        "signers": [
            {
                "account": "NcpevbKeAFioFRs8zdB5J9UkMZQxKBwExw",
                "scopes": "CalledByEntry"
            },
            {
                "account": "NgGCEfyNoSaRZYxoj6WdJqkZnov3DAaY6F",
                "scopes": "CalledByEntry"
            }
        ]
    }
}
```

**Response**

```json
{
    "options": {
        "signers": [
            {
                "account": "NcpevbKeAFioFRs8zdB5J9UkMZQxKBwExw",
                "scopes": "CalledByEntry"
            },
            {
                "account": "NgGCEfyNoSaRZYxoj6WdJqkZnov3DAaY6F",
                "scopes": "CalledByEntry"
            }
        ],
        "signer_metadata": [
            {
                "m": 0,
                "related_accounts": [],
                "signer_account": "NcpevbKeAFioFRs8zdB5J9UkMZQxKBwExw"
            },
            {
                "m": 0,
                "related_accounts": [],
                "signer_account": "NgGCEfyNoSaRZYxoj6WdJqkZnov3DAaY6F"
            }
        ],
        "has_existing_script": false,
        "operations_script": "DAdyb3NldHRhEQwUJphuXfRMjnu\u002BP6CnXX0mjFOIlkIMFN81uo7/y3DFh7uOv63wh29eR03oFMAfDAh0cmFuc2ZlcgwU9WPqQLwoPU0OBcSOowWz8qBzQO9BYn1bUg=="
    },
    "required_public_keys": [
        {
            "address": "NcpevbKeAFioFRs8zdB5J9UkMZQxKBwExw"
        },
        {
            "address": "NgGCEfyNoSaRZYxoj6WdJqkZnov3DAaY6F"
        }
    ]
}
```
