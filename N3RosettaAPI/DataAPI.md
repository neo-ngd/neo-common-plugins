# Neo Rosetta API Plugin | Data API

## Overview

The data APIs can be used to retrieve network, account, block, transaction and other related information from the blockchain. The available APIs are classified based on the endpoints and listed below.

## APIs

### Network

#### Retrieve the list of available networks

Method: **POST**

URL: `/network/list`

Sample request:

```json
{
    "metadata": {}
}
```

Sample response:

```json
{
    "network_identifiers": [
        {
            "blockchain": "neo n3",
            "network": "privatenet"
        }
    ]
}
```

#### Retrieve the network options

Method: **POST**

URL: `/network/options`

Sample request:

```json
{
    "network_identifier": {
        "blockchain": "neo n3",
        "network": "privatenet"
    },
    "metadata": {}
}
```

Sample response:

```json
{
    "version": {
        "rosetta_version": "1.4.10",
        "node_version": "/Neo:3.0.3/"
    },
    "allow": {
        "operation_statuses": [
            {
                "status": "SUCCESS",
                "successful": "true"
            },
            {
                "status": "FAILED",
                "successful": "false"
            }
        ],
        "operation_types": [
            "Transfer"
        ],
        "errors": [
            {
                "code": "1000",
                "message": "the network identifier is invalid",
                "retriable": false
            },
            {
                "code": "2000",
                "message": "the account identifier is invalid",
                "retriable": false
            },
            {
                "code": "2001",
                "message": "the address is invalid",
                "retriable": false
            },
            {
                "code": "2002",
                "message": "account not found",
                "retriable": false
            },
            {
                "code": "2003",
                "message": "the contract address is invalid",
                "retriable": false
            },
            {
                "code": "3000",
                "message": "the block identifier is invalid",
                "retriable": false
            },
            {
                "code": "3001",
                "message": "the block index is invalid",
                "retriable": false
            },
            {
                "code": "3002",
                "message": "the block hash is invalid",
                "retriable": false
            },
            {
                "code": "3003",
                "message": "block not found",
                "retriable": false
            },
            {
                "code": "4000",
                "message": "the transaction identifier is invalid",
                "retriable": false
            },
            {
                "code": "4001",
                "message": "the transaction hash is invalid",
                "retriable": false
            },
            {
                "code": "4002",
                "message": "transaction not found",
                "retriable": false
            },
            {
                "code": "5001",
                "message": "transaction deserialization failed",
                "retriable": false
            },
            {
                "code": "5002",
                "message": "the transaction is already signed",
                "retriable": false
            },
            {
                "code": "5003",
                "message": "no signature is passed in",
                "retriable": false
            },
            {
                "code": "5004",
                "message": "one or more signatures are invalid",
                "retriable": false
            },
            {
                "code": "5005",
                "message": "the curve type is not supported",
                "retriable": false
            },
            {
                "code": "5006",
                "message": "the public key is invalid",
                "retriable": false
            },
            {
                "code": "5007",
                "message": "the transaction witnesses are invalid",
                "retriable": false
            },
            {
                "code": "5008",
                "message": "the transaction metadata is invalid",
                "retriable": false
            },
            {
                "code": "5010",
                "message": "unknown error",
                "retriable": false
            },
            {
                "code": "5011",
                "message": "the transaction already exists and cannot be sent repeatedly",
                "retriable": false
            },
            {
                "code": "5012",
                "message": "the memory pool is full and no more transactions can be sent",
                "retriable": true
            },
            {
                "code": "5013",
                "message": "the transaction cannot be verified",
                "retriable": false
            },
            {
                "code": "5014",
                "message": "the transaction is invalid",
                "retriable": false
            },
            {
                "code": "5015",
                "message": "the transaction is expired",
                "retriable": false
            },
            {
                "code": "5016",
                "message": "the transaction is failed to verify due to insufficient fees",
                "retriable": false
            },
            {
                "code": "5017",
                "message": "one of the policy filters failed",
                "retriable": false
            },
            {
                "code": "6000",
                "message": "parse request body failed",
                "retriable": false
            },
            {
                "code": "6001",
                "message": "one or more params are invalid",
                "retriable": false
            },
            {
                "code": "6002",
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

#### Retrieve the network status

Method: **POST**

URL: `/network/status`

Sample request:

```json
{
    "network_identifier": {
        "blockchain": "neo n3",
        "network": "privatenet"
    },
    "metadata": {}
}
```

Sample response:

```json
{
    "current_block_identifier": {
        "index": "1194",
        "hash": "0x083699b6b48ca00f5502a4ed99eb6fdea76b63f0c9601e541a576ddb4abee650"
    },
    "current_block_timestamp": "1636626629969",
    "genesis_block_identifier": {
        "index": "0",
        "hash": "0x8f2bc69b080918149713368d259dcb3b736f53fe3e8bfe74b5a13c788286c83f"
    },
    "peers": [
        {
            "peer_id": "0x3de8255f6578b7afd1e5433ea48d20c7e3d2cab6",
            "metadata": {
                "connected": "true",
                "address": "127.0.0.1:30333",
                "height": "1195"
            }
        }
    ]
}
```

### Account

#### Retrieve account balance

Method: **POST**

URL: `/account/balance`

##### Providing a currency list to the endpoint, it returns corresponding currency balances.

Sample request:

```json
{
    "network_identifier": {
        "blockchain": "neo n3",
        "network": "privatenet"
    },
    "account_identifier": {
        "address": "NfdK3KadbACwvHKZ5ygCsZa3MNgmucFUN6",
        "metadata": {}
    },
    "currencies": [
        {
            "symbol": "gas",
            "decimals": 8,
            "metadata": {
                "script_hash": "0xd2a4cff31913016155e38e474a2c06d08be276cf"
            }
        }
    ]
}
```

Sample response:

```json
{
    "block_identifier": {
        "index": "1208",
        "hash": "0x3d99ec92ffadf31b442f7a86fd1fcf091fa2405708406f55aabb6c2e5d51fe3c"
    },
    "balances": [
        {
            "value": "1000000000000000",
            "currency": {
                "symbol": "GAS",
                "decimals": "8",
                "metadata": {
                    "script_hash": "0xd2a4cff31913016155e38e474a2c06d08be276cf"
                }
            }
        }
    ]
}
```

##### Leaving the currency list empty, the endpoint returns NEO and GAS balances.

Sample request:

```json
{
    "network_identifier": {
        "blockchain": "neo n3",
        "network": "privatenet"
    },
    "account_identifier": {
        "address": "NfdK3KadbACwvHKZ5ygCsZa3MNgmucFUN6",
        "metadata": {}
    }
}
```

Sample response:

```json
{
    "block_identifier": {
        "index": "1208",
        "hash": "0x3d99ec92ffadf31b442f7a86fd1fcf091fa2405708406f55aabb6c2e5d51fe3c"
    },
    "balances": [
        {
            "value": "100000000",
            "currency": {
                "symbol": "NEO",
                "decimals": "0",
                "metadata": {
                    "script_hash": "0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5"
                }
            }
        },
        {
            "value": "1000000000000000",
            "currency": {
                "symbol": "GAS",
                "decimals": "8",
                "metadata": {
                    "script_hash": "0xd2a4cff31913016155e38e474a2c06d08be276cf"
                }
            }
        }
    ]
}
```

The `currency` field contains token details. The two tokens in neo network are denoted in the following manner:

**NEO** token:

```json
"currency": {
    "symbol": "NEO",
    "decimals": "0",
    "metadata": {
        "script_hash": "0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5"
    }
}
```

**GAS** token:

```json
"currency": {
    "symbol": "GAS",
    "decimals": "8",
    "metadata": {
        "script_hash": "0xd2a4cff31913016155e38e474a2c06d08be276cf"
    }
}
```

### Block

#### Retrieve a specific block details

Method: **POST**

URL: `/block`

Sample request:

```json
{
    "network_identifier": {
        "blockchain": "neo n3",
        "network": "privatenet"
    },
    "block_identifier": {
        "index": 1344,
        "hash": ""
    }
}
```

Sample response:

```json
{
    "block": {
        "block_identifier": {
            "index": "1344",
            "hash": "0xa39bc8c7f9ce0f9af33a374045569b5fa59dc7e0d85377bd1be11248bf552616"
        },
        "parent_block_identifier": {
            "index": "1343",
            "hash": "0x7f086b364b631dd6a0176d4ed40400164854ffed3421536fa1d5b6bad8270f15"
        },
        "timestamp": "1636689423195000",
        "transactions": [
            {
                "transaction_identifier": {
                    "hash": "0xf9d70944afbe9f1839c9067e7d5b50f9bb2b5073a161df82e3efb6d31cc8c708"
                },
                "operations": [
                    {
                        "operation_identifier": {
                            "index": "0"
                        },
                        "type": "Transfer",
                        "status": "SUCCESS",
                        "account": {
                            "address": "NfdK3KadbACwvHKZ5ygCsZa3MNgmucFUN6"
                        },
                        "amount": {
                            "value": "-10000000000",
                            "currency": {
                                "symbol": "GAS",
                                "decimals": "8",
                                "metadata": {
                                    "script_hash": "0xd2a4cff31913016155e38e474a2c06d08be276cf"
                                }
                            }
                        }
                    },
                    {
                        "operation_identifier": {
                            "index": "1"
                        },
                        "related_operations": [
                            {
                                "index": "0"
                            }
                        ],
                        "type": "Transfer",
                        "status": "SUCCESS",
                        "account": {
                            "address": "NQdAHrGdt7QgMhNFUv5AuCYCKtu4cvTHeQ"
                        },
                        "amount": {
                            "value": "10000000000",
                            "currency": {
                                "symbol": "GAS",
                                "decimals": "8",
                                "metadata": {
                                    "script_hash": "0xd2a4cff31913016155e38e474a2c06d08be276cf"
                                }
                            }
                        }
                    }
                ],
                "metadata": {
                    "size": 253,
                    "version": 0,
                    "nonce": 1800434103,
                    "sender": "NfdK3KadbACwvHKZ5ygCsZa3MNgmucFUN6",
                    "sysfee": "9977780",
                    "netfee": "1236520",
                    "validuntilblock": 7103,
                    "signers": [
                        {
                            "account": "0x589e5b84d2e0e206c63a34415aa79064a1043cd8",
                            "scopes": "CalledByEntry"
                        }
                    ],
                    "attributes": [],
                    "script": "CwMA5AtUAgAAAAwUM6rT3sgbq+Y6iNAP8Fd4GHEBg0cMFNg8BKFkkKdaQTQ6xgbi4NKEW55YFMAfDAh0cmFuc2ZlcgwUz3bii9AGLEpHjuNVYQETGfPPpNJBYn1bUjk=",
                    "witnesses": [
                        {
                            "invocation": "DECJq2MZpxHc+hcPgsD6pdNlfTlQh1oJWV4tLrnurobCrUmm1z0PDYkhc3YF0Sx/AE1zbnuyA+G2a/6CoIggtR0M",
                            "verification": "DCECRu4f74Z307xiQlYgTbMiOw0LvZYP1j7+/UwQXIgoqoxBVuezJw=="
                        }
                    ],
                    "blockhash": "0xa39bc8c7f9ce0f9af33a374045569b5fa59dc7e0d85377bd1be11248bf552616",
                    "blocktime": 1636689423195
                }
            }
        ]
    }
}
```

#### Retrieve a specific transaction details in a specific block

Method: **POST**

URL: `/block/transaction`

Sample request:

```json
{
    "network_identifier": {
        "blockchain": "neo n3",
        "network": "privatenet"
    },
    "block_identifier": {
        "index": 1344,
        "hash": "0xa39bc8c7f9ce0f9af33a374045569b5fa59dc7e0d85377bd1be11248bf552616"
    },
    "transaction_identifier": {
        "hash": "0xf9d70944afbe9f1839c9067e7d5b50f9bb2b5073a161df82e3efb6d31cc8c708"
    }
}
```

Sample response:

```json
{
    "transaction": {
        "transaction_identifier": {
            "hash": "0xf9d70944afbe9f1839c9067e7d5b50f9bb2b5073a161df82e3efb6d31cc8c708"
        },
        "operations": [
            {
                "operation_identifier": {
                    "index": "0"
                },
                "type": "Transfer",
                "status": "SUCCESS",
                "account": {
                    "address": "NfdK3KadbACwvHKZ5ygCsZa3MNgmucFUN6"
                },
                "amount": {
                    "value": "-10000000000",
                    "currency": {
                        "symbol": "GAS",
                        "decimals": "8",
                        "metadata": {
                            "script_hash": "0xd2a4cff31913016155e38e474a2c06d08be276cf"
                        }
                    }
                }
            },
            {
                "operation_identifier": {
                    "index": "1"
                },
                "related_operations": [
                    {
                        "index": "0"
                    }
                ],
                "type": "Transfer",
                "status": "SUCCESS",
                "account": {
                    "address": "NQdAHrGdt7QgMhNFUv5AuCYCKtu4cvTHeQ"
                },
                "amount": {
                    "value": "10000000000",
                    "currency": {
                        "symbol": "GAS",
                        "decimals": "8",
                        "metadata": {
                            "script_hash": "0xd2a4cff31913016155e38e474a2c06d08be276cf"
                        }
                    }
                }
            }
        ],
        "metadata": {
            "size": 253,
            "version": 0,
            "nonce": 1800434103,
            "sender": "NfdK3KadbACwvHKZ5ygCsZa3MNgmucFUN6",
            "sysfee": "9977780",
            "netfee": "1236520",
            "validuntilblock": 7103,
            "signers": [
                {
                    "account": "0x589e5b84d2e0e206c63a34415aa79064a1043cd8",
                    "scopes": "CalledByEntry"
                }
            ],
            "attributes": [],
            "script": "CwMA5AtUAgAAAAwUM6rT3sgbq+Y6iNAP8Fd4GHEBg0cMFNg8BKFkkKdaQTQ6xgbi4NKEW55YFMAfDAh0cmFuc2ZlcgwUz3bii9AGLEpHjuNVYQETGfPPpNJBYn1bUjk=",
            "witnesses": [
                {
                    "invocation": "DECJq2MZpxHc+hcPgsD6pdNlfTlQh1oJWV4tLrnurobCrUmm1z0PDYkhc3YF0Sx/AE1zbnuyA+G2a/6CoIggtR0M",
                    "verification": "DCECRu4f74Z307xiQlYgTbMiOw0LvZYP1j7+/UwQXIgoqoxBVuezJw=="
                }
            ],
            "blockhash": "0xa39bc8c7f9ce0f9af33a374045569b5fa59dc7e0d85377bd1be11248bf552616",
            "blocktime": 1636689423195
        }
    }
}
```

Please note that the transaction record consists of **two** operations, i.e. the from operation, and the to operation. The transaction amount for the **from** operation is **negative**, while the amount for the **to** operation is **positive**.

### Mempool

#### Retrieve all transactions in the mempool

Method: **POST**

URL: `/mempool`

Sample request:

```json
{
    "network_identifier": {
        "blockchain": "neo n3",
        "network": "privatenet"
    },
    "metadata": {}
}
```

Sample response:

```json
{
    "transaction_identifiers": [
        {
            "hash": "0x7a0608136e015999849c3bd0e408179496045369ca91f966c82f379fbed46266"
        }
    ]
}
```

#### Retrieve a specific transaction details in the mempool

Method: **POST**

URL: `/mempool/transaction`

Sample request:

```json
{
    "network_identifier": {
        "blockchain": "neo n3",
        "network": "privatenet"
    },
    "transaction_identifier": {
        "hash": "0x7a0608136e015999849c3bd0e408179496045369ca91f966c82f379fbed46266"
    }
}
```

Sample response:

```json
{
    "transaction": {
        "transaction_identifier": {
            "hash": "0x7a0608136e015999849c3bd0e408179496045369ca91f966c82f379fbed46266"
        },
        "operations": [],
        "metadata": {
            "size": 253,
            "version": 0,
            "nonce": 480579460,
            "sysfee": 9977780,
            "netfee": 1236520,
            "sender": "NfdK3KadbACwvHKZ5ygCsZa3MNgmucFUN6",
            "script": "CwMAyBeoBAAAAAwUM6rT3sgbq+Y6iNAP8Fd4GHEBg0cMFNg8BKFkkKdaQTQ6xgbi4NKEW55YFMAfDAh0cmFuc2ZlcgwUz3bii9AGLEpHjuNVYQETGfPPpNJBYn1bUjk=",
            "signers": [
                {
                    "account": "0x589e5b84d2e0e206c63a34415aa79064a1043cd8",
                    "scopes": "CalledByEntry"
                }
            ],
            "attributes": [],
            "witnesses": [
                {
                    "invocation": "DEA4A2jDyxZ0hTQDfYhEUchKjIcIX43sD5t1ETFxtRLDSdokWGLs+1mB+SjEdOTmHKJ0WuRXTIJDz/Hl0dELsvaU",
                    "verification": "DCECRu4f74Z307xiQlYgTbMiOw0LvZYP1j7+/UwQXIgoqoxBVuezJw=="
                }
            ]
        }
    }
}
```
