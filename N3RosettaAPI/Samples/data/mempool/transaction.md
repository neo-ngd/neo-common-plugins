**Request**

```json
{
    "network_identifier": {
        "blockchain": "neo n3",
        "network": "testnet"
    },
    "transaction_identifier": {
        "hash": "0x999458efc07e2fd1504a27acf7cadaf48ec3f134449f6ad9523809e10e86fd27"
    }
}
```

**Response**

```json
{
    "transaction": {
        "transaction_identifier": {
            "hash": "0x999458efc07e2fd1504a27acf7cadaf48ec3f134449f6ad9523809e10e86fd27"
        },
        "operations": [
            {
                "operation_identifier": {
                    "index": 0
                },
                "type": "Transfer",
                "status": "SUCCESS",
                "account": {
                    "address": "Nc5MTVqB1YdVhpf4zHLGdzGFeQEUmmrQ1R"
                },
                "amount": {
                    "value": "50000000",
                    "currency": {
                        "symbol": "GAS",
                        "decimals": 8,
                        "metadata": {
                            "script_hash": "0xd2a4cff31913016155e38e474a2c06d08be276cf"
                        }
                    }
                }
            }
        ]
    }
}
```
