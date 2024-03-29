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
        "netfee": 234804,
        "nonce": 296711092,
        "script": "DAdyb3NldHRhEQwUJphuXfRMjnu\u002BP6CnXX0mjFOIlkIMFN81uo7/y3DFh7uOv63wh29eR03oFMAfDAh0cmFuc2ZlcgwU9WPqQLwoPU0OBcSOowWz8qBzQO9BYn1bUg==",
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
        ],
        "sysfee": 997796,
        "validuntilblock": 1428723,
        "version": 0
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
    "unsigned_transaction": "ALRzrxGkOQ8AAAAAADSVAwAAAAAA88wVAAK5eNCL546SGQS2/lr79rjoWq9gnQHfNbqO/8twxYe7jr\u002Bt8IdvXkdN6AEAXgwHcm9zZXR0YREMFCaYbl30TI57vj\u002Bgp119JoxTiJZCDBTfNbqO/8twxYe7jr\u002Bt8IdvXkdN6BTAHwwIdHJhbnNmZXIMFPVj6kC8KD1NDgXEjqMFs/Kgc0DvQWJ9W1ICACgMIQNgpkd/JmKLheTo22NOCBMwXuJ9eH5dVbX33XUjgO47\u002B0FW57MnACgMIQKw4cVA3wJMYxCqoePYVa2fzodCbSRdt1/qNE41kTyPzkFW57Mn",
    "payloads": [
        {
            "account_identifier": {
                "address": "NcpevbKeAFioFRs8zdB5J9UkMZQxKBwExw"
            },
            "hex_bytes": "b1982e918ec252b6b0687eed95e774804337380df1e7095443b16cc9db34444a",
            "signature_type": "ecdsa"
        },
        {
            "account_identifier": {
                "address": "NgGCEfyNoSaRZYxoj6WdJqkZnov3DAaY6F"
            },
            "hex_bytes": "b1982e918ec252b6b0687eed95e774804337380df1e7095443b16cc9db34444a",
            "signature_type": "ecdsa"
        }
    ]
}
```
