**Request**

```json
{
    "network_identifier": {
        "blockchain": "neo n3",
        "network": "testnet"
    },
    "block_identifier": {
        "index": 0
    }
}
```

**Response**

```json
{
    "block": {
        "block_identifier": {
            "index": 0,
            "hash": "0x9d3276785e7306daf59a3f3b9e31912c095598bbfb8a4476b821b0e59be4c57a"
        },
        "parent_block_identifier": {
            "index": 0,
            "hash": "0x9d3276785e7306daf59a3f3b9e31912c095598bbfb8a4476b821b0e59be4c57a"
        },
        "timestamp": 1468595301000000,
        "transactions": [
            {
                "transaction_identifier": {
                    "hash": "0x9d3276785e7306daf59a3f3b9e31912c095598bbfb8a4476b821b0e59be4c57a"
                },
                "operations": [
                    {
                        "operation_identifier": {
                            "index": 0
                        },
                        "type": "Transfer",
                        "status": "SUCCESS",
                        "account": {
                            "address": "NZHf1NJvz1tvELGLWZjhpb3NqZJFFUYpxT"
                        },
                        "amount": {
                            "value": "100000000",
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
                        "type": "Transfer",
                        "status": "SUCCESS",
                        "account": {
                            "address": "NZHf1NJvz1tvELGLWZjhpb3NqZJFFUYpxT"
                        },
                        "amount": {
                            "value": "5200000000000000",
                            "currency": {
                                "symbol": "GAS",
                                "decimals": 8,
                                "metadata": {
                                    "script_hash": "0xd2a4cff31913016155e38e474a2c06d08be276cf"
                                }
                            }
                        }
                    },
                    {
                        "operation_identifier": {
                            "index": 2
                        },
                        "type": "Transfer",
                        "status": "SUCCESS",
                        "account": {
                            "address": "NL4PXTc8dxjBca8FEkJYCEgDWL98ZnzcnV"
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
        ]
    }
}
```
