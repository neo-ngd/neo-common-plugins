**Request**

```json
{
    "network_identifier": {
        "blockchain": "neo n3",
        "network": "testnet"
    },
    "options": {
        "has_existing_script": false,
        "operations_script": "DAdyb3NldHRhEQwUJphuXfRMjnu\u002BP6CnXX0mjFOIlkIMFN81uo7/y3DFh7uOv63wh29eR03oFMAfDAh0cmFuc2ZlcgwU9WPqQLwoPU0OBcSOowWz8qBzQO9BYn1bUg==",
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
    },
    "public_keys": [
        {
            "hex_bytes": "0460a6477f26628b85e4e8db634e0813305ee27d787e5d55b5f7dd752380ee3bfb87d23c2bf9f7f2cd4c3f8e0dcdfc3846b9b902da26bc970e01732e06b6101ae7",
            "curve_type": "secp256r1"
        },
        {
            "hex_bytes": "04b0e1c540df024c6310aaa1e3d855ad9fce87426d245db75fea344e35913c8fce11ac4ce19c1af2f566039c6a76b05ea25fe746602cbfcefd278b24d031a19020",
            "curve_type": "secp256r1"
        }
    ]
}
```

**Response**

```json
{
    "metadata": {
        "version": 0,
        "nonce": 296711092,
        "sysfee": 997796,
        "netfee": 234804,
        "validuntilblock": 1428723,
        "script": "DAdyb3NldHRhEQwUJphuXfRMjnu\u002BP6CnXX0mjFOIlkIMFN81uo7/y3DFh7uOv63wh29eR03oFMAfDAh0cmFuc2ZlcgwU9WPqQLwoPU0OBcSOowWz8qBzQO9BYn1bUg==",
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
        ]
    },
    "suggested_fee": [
        {
            "value": "1232600",
            "currency": {
                "symbol": "GAS",
                "decimals": 8,
                "metadata": {
                    "script_hash": "0xd2a4cff31913016155e38e474a2c06d08be276cf"
                }
            }
        }
    ]
}
```
