**Request**

```json
{
    "network_identifier": {
        "blockchain": "neo n3",
        "network": "testnet"
    },
    "signed": false,
    "transaction": "ALRzrxGkOQ8AAAAAADSVAwAAAAAA88wVAAK5eNCL546SGQS2/lr79rjoWq9gnQHfNbqO/8twxYe7jr\u002Bt8IdvXkdN6AEAXgwHcm9zZXR0YREMFCaYbl30TI57vj\u002Bgp119JoxTiJZCDBTfNbqO/8twxYe7jr\u002Bt8IdvXkdN6BTAHwwIdHJhbnNmZXIMFPVj6kC8KD1NDgXEjqMFs/Kgc0DvQWJ9W1ICACgMIQNgpkd/JmKLheTo22NOCBMwXuJ9eH5dVbX33XUjgO47\u002B0FW57MnACgMIQKw4cVA3wJMYxCqoePYVa2fzodCbSRdt1/qNE41kTyPzkFW57Mn"
}
```

**Response**

```json
{
    "operations": [
        {
            "operation_identifier": {
                "index": 0
            },
            "type": "Transfer",
            "status": null,
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
            "status": null,
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
    "signers": [],
    "metadata": {
        "size": 165,
        "version": 0,
        "nonce": 296711092,
        "sysfee": 997796,
        "netfee": 234804,
        "sender": "NcpevbKeAFioFRs8zdB5J9UkMZQxKBwExw",
        "script": "DAdyb3NldHRhEQwUJphuXfRMjnu\u002BP6CnXX0mjFOIlkIMFN81uo7/y3DFh7uOv63wh29eR03oFMAfDAh0cmFuc2ZlcgwU9WPqQLwoPU0OBcSOowWz8qBzQO9BYn1bUg==",
        "signers": [
            {
                "account": "0x9d60af5ae8b8f6fb5afeb60419928ee78bd078b9",
                "scopes": "CalledByEntry"
            },
            {
                "account": "0xe84d475e6f87f0adbf8ebb87c570cbff8eba35df",
                "scopes": "CalledByEntry"
            }
        ],
        "attributes": [],
        "witnesses": []
    }
}
```
