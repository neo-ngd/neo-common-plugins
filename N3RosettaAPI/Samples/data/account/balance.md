**Request**

```json
{
    "network_identifier": {
        "blockchain": "neo n3",
        "network": "testnet"
    },
    "block_identifier": {
        "index": 0
    },
    "account_identifier": {
        "address": "NZHf1NJvz1tvELGLWZjhpb3NqZJFFUYpxT",
        "metadata": {}
    },
    "currencies": [
        {
            "symbol": "NEO",
            "decimals": 0,
            "metadata": {
                "script_hash": "0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5"
            }
        }
    ],
    "metadata": {}
}
```

**Response**

```json
{
    "block_identifier": {
        "index": 0,
        "hash": "0x9d3276785e7306daf59a3f3b9e31912c095598bbfb8a4476b821b0e59be4c57a"
    },
    "balances": [
        {
            "value": "100000000",
            "currency": {
                "symbol": "NEO",
                "decimals": 0,
                "metadata": {
                    "script_hash": "0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5"
                }
            }
        }
    ]
}
```
