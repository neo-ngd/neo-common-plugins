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
            "blockchain": "neo",
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
        "blockchain": "neo",
        "network": "privatenet"
    },
    "metadata": {}
}
```

Sample response:

```json
{
    "version": {
        "rosetta_version": "1.4.1",
        "node_version": "/Neo:2.10.3/"
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
                "message": "network identifier is invalid",
                "retriable": "false"
            },
            {
                "code": "2000",
                "message": "account identifier is invalid",
                "retriable": "false"
            },
            {
                "code": "2001",
                "message": "address is invalid",
                "retriable": "false"
            },
            {
                "code": "2002",
                "message": "account not found",
                "retriable": "false"
            },
            {
                "code": "2003",
                "message": "contract address is invalid",
                "retriable": "false"
            },
            {
                "code": "3000",
                "message": "block identifier is invalid",
                "retriable": "false"
            },
            {
                "code": "3001",
                "message": "block index is invalid",
                "retriable": "false"
            },
            {
                "code": "3002",
                "message": "block hash is invalid",
                "retriable": "false"
            },
            {
                "code": "3003",
                "message": "block not found",
                "retriable": "false"
            },
            {
                "code": "4000",
                "message": "transaction identifier is invalid",
                "retriable": "false"
            },
            {
                "code": "4001",
                "message": "transaction hash is invalid",
                "retriable": "false"
            },
            {
                "code": "4002",
                "message": "transaction not found",
                "retriable": "false"
            },
            {
                "code": "5001",
                "message": "transaction deserialization error",
                "retriable": "false"
            },
            {
                "code": "5002",
                "message": "transaction is already signed",
                "retriable": "false"
            },
            {
                "code": "5003",
                "message": "no signature is passed in",
                "retriable": "false"
            },
            {
                "code": "5004",
                "message": "one or more signatures are invalid",
                "retriable": "false"
            },
            {
                "code": "5005",
                "message": "curve type is not supported",
                "retriable": "false"
            },
            {
                "code": "5006",
                "message": "public key is invalid",
                "retriable": "false"
            },
            {
                "code": "5007",
                "message": "transaction witness is invalid",
                "retriable": "false"
            },
            {
                "code": "5008",
                "message": "transaction metadata is invalid",
                "retriable": "false"
            },
            {
                "code": "5010",
                "message": "Unknown error.",
                "retriable": "false"
            },
            {
                "code": "5011",
                "message": "The transaction already exists and cannot be sent repeatedly.",
                "retriable": "false"
            },
            {
                "code": "5012",
                "message": "The memory pool is full and no more transactions can be sent.",
                "retriable": "true"
            },
            {
                "code": "5013",
                "message": "The transaction cannot be verified.",
                "retriable": "false"
            },
            {
                "code": "5014",
                "message": "The transaction is invalid.",
                "retriable": "false"
            },
            {
                "code": "5015",
                "message": "One of the policy filters failed.",
                "retriable": "false"
            },
            {
                "code": "6000",
                "message": "parse request body error",
                "retriable": "false"
            },
            {
                "code": "6001",
                "message": "one or more params are invalid",
                "retriable": "false"
            },
            {
                "code": "6002",
                "message": "engine faulted",
                "retriable": "false"
            }
        ],
        "historical_balance_lookup": "false"
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
        "blockchain": "neo",
        "network": "privatenet"
    },
    "metadata": {}
}
```

Sample response:

```json
{
    "current_block_identifier": {
        "index": "59811",
        "hash": "0x71b8f6d4950b7820d12d54b3eb30474674d84b3d854ea694f09753d5ab26d52e"
    },
    "current_block_timestamp": "722777480",
    "genesis_block_identifier": {
        "index": "0",
        "hash": "0x7d86a70c6f3162ae4adc1afe6a3e67c83caa043379aeca0d0fd4e06c4a328358"
    },
    "peers": [
        {
            "peer_id": "0x34b26bdbca3291b066b2e494659409cab97ebd0f",
            "metadata": {
                "connected": "true",
                "address": "127.0.0.1:20003",
                "height": "59811"
            }
        }
    ]
}
```

### Account

#### Retrieve account balance

Method: **POST**

URL: `/account/balance`

Sample request:

```json
{
    "network_identifier": {
        "blockchain": "neo",
        "network": "privatenet"
    },
    "account_identifier": {
        "address": "AQzRMe3zyGS8W177xLJfewRRQZY2kddMun",
        "sub_account": {
            "address": "0xf7d657c10ab725ae54f80230d93fc0317b8ee3e8",
            "metadata": {}
        },
        "metadata": {}
    },
    "block_identifier": {
        "index": 1123941,
        "hash": "0x1f2cc6c5027d2f201a5453ad1119574d2aed23a392654742ac3c78783c071f85"
    }
}
```

Sample response:

```json
{
    "block_identifier": {
        "index": "59811",
        "hash": "0x71b8f6d4950b7820d12d54b3eb30474674d84b3d854ea694f09753d5ab26d52e"
    },
    "balances": [
        {
            "value": "100000000",
            "currency": {
                "symbol": "NEO",
                "decimals": "0",
                "metadata": {
                    "token_type": "Governing Token"
                }
            }
        },
        {
            "value": "9378",
            "currency": {
                "symbol": "GAS",
                "decimals": "8",
                "metadata": {
                    "token_type": "Utility Token"
                }
            }
        },
        {
            "value": "9999999964980000",
            "currency": {
                "symbol": "MNF",
                "decimals": "8",
                "metadata": {
                    "token_type": "NEP5 Token"
                }
            }
        }
    ]
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
        "blockchain": "neo",
        "network": "privatenet"
    },
    "block_identifier": {
        "index": 14653,
        "hash": "0x10c2b9438c08fbeb96db702820e9530f5514cbcd38d51b4424e581e5e0799016"
    }
}
```

Sample response:

```json
{
    "block": {
        "block_identifier": {
            "index": "14653",
            "hash": "0x10c2b9438c08fbeb96db702820e9530f5514cbcd38d51b4424e581e5e0799016"
        },
        "parent_block_identifier": {
            "index": "14652",
            "hash": "0xaf68048ec2c488782df9815d3e4468c2433f0a14c88f6677426fb3aa5247bb48"
        },
        "timestamp": "3358516072",
        "transactions": [
            {
                "transaction_identifier": {
                    "hash": "0x807d4ba01f3ff436fe777b9c364408fda112b7479b09f5050e8ce6d7778251c1"
                },
                "operations": [
                    {
                        "operation_identifier": {
                            "index": "0"
                        },
                        "type": "Transfer",
                        "status": "SUCCESS",
                        "account": {
                            "address": "AQzRMe3zyGS8W177xLJfewRRQZY2kddMun"
                        },
                        "amount": {
                            "value": "-18636.9536",
                            "currency": {
                                "symbol": "GAS",
                                "decimals": "8",
                                "metadata": {
                                    "token_type": "Utility Token"
                                }
                            }
                        },
                        "coin_change": {
                            "coin_identifier": {
                                "identifier": "0x04aa38159833f8d3c970be0f080f06f8d0fdbe4e4b066e18f490a1863dcefe52:0"
                            },
                            "coin_action": "coin_spent"
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
                            "address": "AQzRMe3zyGS8W177xLJfewRRQZY2kddMun"
                        },
                        "amount": {
                            "value": "17645.9536",
                            "currency": {
                                "symbol": "GAS",
                                "decimals": "8",
                                "metadata": {
                                    "token_type": "Utility Token"
                                }
                            }
                        },
                        "coin_change": {
                            "coin_identifier": {
                                "identifier": "0x807d4ba01f3ff436fe777b9c364408fda112b7479b09f5050e8ce6d7778251c1:0"
                            },
                            "coin_action": "coin_created"
                        }
                    }
                ],
                "metadata": {
                    "script": "027373027373027373027373027373570105020710142f885215685d2bdc4785cc2d79a0f213fe0226f759c1077570677261646567a6d9620518da3f006a7ab91f224c228cba956642",
                    "gas": "991",
                    "tx_type": "InvocationTransaction"
                }
            }
        ]
    },
    "other_transactions": [
        {
            "hash": "0x43de21241724e366b492c172b6e929e508d3d3ec8b7d5ce15a1f3da728798b6c"
        }
    ]
}
```

#### Retrieve a specific transaction details in a specific block

Method: **POST**

URL: `/block/transaction`

Sample request:

```json
{
    "network_identifier": {
        "blockchain": "neo",
        "network": "privatenet"
    },
    "block_identifier": {
        "index": 14653,
        "hash": "0x10c2b9438c08fbeb96db702820e9530f5514cbcd38d51b4424e581e5e0799016"
    },
    "transaction_identifier": {
        "hash": "0x807d4ba01f3ff436fe777b9c364408fda112b7479b09f5050e8ce6d7778251c1"
    }
}
```

Sample response:

```json
{
    "transaction": {
        "transaction_identifier": {
            "hash": "0x807d4ba01f3ff436fe777b9c364408fda112b7479b09f5050e8ce6d7778251c1"
        },
        "operations": [
            {
                "operation_identifier": {
                    "index": "0"
                },
                "type": "Transfer",
                "status": "SUCCESS",
                "account": {
                    "address": "AQzRMe3zyGS8W177xLJfewRRQZY2kddMun"
                },
                "amount": {
                    "value": "-18636.9536",
                    "currency": {
                        "symbol": "GAS",
                        "decimals": "8",
                        "metadata": {
                            "token_type": "Utility Token"
                        }
                    }
                },
                "coin_change": {
                    "coin_identifier": {
                        "identifier": "0x04aa38159833f8d3c970be0f080f06f8d0fdbe4e4b066e18f490a1863dcefe52:0"
                    },
                    "coin_action": "coin_spent"
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
                    "address": "AQzRMe3zyGS8W177xLJfewRRQZY2kddMun"
                },
                "amount": {
                    "value": "17645.9536",
                    "currency": {
                        "symbol": "GAS",
                        "decimals": "8",
                        "metadata": {
                            "token_type": "Utility Token"
                        }
                    }
                },
                "coin_change": {
                    "coin_identifier": {
                        "identifier": "0x807d4ba01f3ff436fe777b9c364408fda112b7479b09f5050e8ce6d7778251c1:0"
                    },
                    "coin_action": "coin_created"
                }
            }
        ],
        "metadata": {
            "script": "027373027373027373027373027373570105020710142f885215685d2bdc4785cc2d79a0f213fe0226f759c1077570677261646567a6d9620518da3f006a7ab91f224c228cba956642",
            "gas": "991",
            "tx_type": "InvocationTransaction"
        }
    }
}
```

Please note that the transaction record consists of **two** operations, i.e. the from operation, and the to operation. The transaction amount for the **from** operation is **negative**, while the amount for the **to** operation is **positive**.

The `currency` field contains token details. The two tokens in neo network are denoted in the following manner:

**NEO** token:

```json
"currency": {
    "symbol": "NEO",
    "decimals": "0",
    "metadata": {
        "token_type": "Governing Token"
    }
}
```

**GAS** token:

```json
"currency": {
    "symbol": "GAS",
    "decimals": "8",
    "metadata": {
        "token_type": "Utility Token"
    }
}
```

### Mempool

#### Retrieve all transactions in the mempool

Method: **POST**

URL: `/mempool`

Sample request:

```json
{
    "network_identifier": {
        "blockchain": "neo",
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
            "hash": "0xd79d229fe054406adc6d7441d0b605d37d00c1c72272a2744db200f87c9ba2c1"
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
        "blockchain": "neo",
        "network": "privatenet"
    },
    "transaction_identifier": {
        "hash": "0xd79d229fe054406adc6d7441d0b605d37d00c1c72272a2744db200f87c9ba2c1"
    }
}
```

Sample response:

```json
{
    "transaction": {
        "transaction_identifier": {
            "hash": "0xd79d229fe054406adc6d7441d0b605d37d00c1c72272a2744db200f87c9ba2c1"
        },
        "operations": [
            {
                "operation_identifier": {
                    "index": "0"
                },
                "type": "Transfer",
                "status": "SUCCESS",
                "account": {
                    "address": "AQzRMe3zyGS8W177xLJfewRRQZY2kddMun"
                },
                "amount": {
                    "value": "-100000000",
                    "currency": {
                        "symbol": "NEO",
                        "decimals": "0",
                        "metadata": {
                            "token_type": "Governing Token"
                        }
                    }
                },
                "coin_change": {
                    "coin_identifier": {
                        "identifier": "0x2e7d4adaa2c7b65a0fa13fae7310dd371774e66f9cd70865bb97c9bbc5c22ff8:0"
                    },
                    "coin_action": "coin_spent"
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
                    "address": "AQzRMe3zyGS8W177xLJfewRRQZY2kddMun"
                },
                "amount": {
                    "value": "100000000",
                    "currency": {
                        "symbol": "NEO",
                        "decimals": "0",
                        "metadata": {
                            "token_type": "Governing Token"
                        }
                    }
                },
                "coin_change": {
                    "coin_identifier": {
                        "identifier": "0xd79d229fe054406adc6d7441d0b605d37d00c1c72272a2744db200f87c9ba2c1:0"
                    },
                    "coin_action": "coin_created"
                }
            }
        ],
        "metadata": {
            "tx_type": "ContractTransaction"
        }
    }
}
```
