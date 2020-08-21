# network
## /network/list
request:
{
    "metadata": {}
}

response:
{
    "network_identifiers": [
        {
            "blockchain": "neo",
            "network": "privatenet"
        }
    ]
}

## /network/options
request:
{
    "network_identifier": {
        "blockchain": "neo",
        "network": "privatenet"
    },
    "metadata": {}
}

response:
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


## /network/status
request:
{
    "network_identifier": {
        "blockchain": "neo",
        "network": "privatenet"
    },
    "metadata": {}
}

response:
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

# account
## /account/balance
request:
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

response:
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

# block
## /block
request:
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

response:
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

## /block/transaction
request:
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

response:
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

# mempool
## /mempool
request:
{
    "network_identifier": {
        "blockchain": "neo",
        "network": "privatenet"
    },
    "metadata": {}
}

response:
{
    "transaction_identifiers": [
        {
            "hash": "0xd79d229fe054406adc6d7441d0b605d37d00c1c72272a2744db200f87c9ba2c1"
        }
    ]
}

## /mempool/transaction
request:
{
    "network_identifier": {
        "blockchain": "neo",
        "network": "privatenet"
    },
    "transaction_identifier": {
        "hash": "0xd79d229fe054406adc6d7441d0b605d37d00c1c72272a2744db200f87c9ba2c1"
    }
}

response:
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

# construction

## Flow of Operations
                               Caller (i.e. Coinbase)                + Construction API Implementation
                              +-------------------------------------------------------------------------------------------+
                                                                     |
                               Derive Address   +----------------------------> /construction/derive
                               from Public Key                       |
                                                                     |
                             X                                       |
                             X Create Metadata Request +---------------------> /construction/preprocess
                             X (array of operations)                 |                    +
    Get metadata needed      X                                       |                    |
    to construct transaction X            +-----------------------------------------------+
                             X            v                          |
                             X Fetch Online Metadata +-----------------------> /construction/metadata (online)
                             X                                       |
                                                                     |
                             X                                       |
                             X Construct Payloads to Sign +------------------> /construction/payloads
                             X (array of operations)                 |                   +
                             X                                       |                   |
 Create unsigned transaction X          +------------------------------------------------+
                             X          v                            |
                             X Parse Unsigned Transaction +------------------> /construction/parse
                             X to Confirm Correctness                |
                             X                                       |
                                                                     |
                             X                                       |
                             X Sign Payload(s) +-----------------------------> /construction/combine
                             X (using caller's own detached signer)  |                 +
                             X                                       |                 |
   Create signed transaction X         +-----------------------------------------------+
                             X         v                             |
                             X Parse Signed Transaction +--------------------> /construction/parse
                             X to Confirm Correctness                |
                             X                                       |
                                                                     |
                             X                                       |
                             X Get hash of signed transaction +--------------> /construction/hash
Broadcast Signed Transaction X to monitor status                     |
                             X                                       |
                             X Submit Transaction +--------------------------> /construction/submit (online)
                             X                                       |
                                                                     +

## /construction/derive
request:
{
    "network_identifier": {
        "blockchain": "neo",
        "network": "privatenet"
    },
    "public_key": {
        "hex_bytes": "03d08d6f766b54e35745bc99d643c939ec6f3d37004f2a59006be0e53610f0be25",
        "curve_type": "secp256r1"
    },
    "metadata": {}
}

response:
{
    "address": "AQzRMe3zyGS8W177xLJfewRRQZY2kddMun"
}

## /construction/preprocess
request:
{
    "network_identifier": {
        "blockchain": "neo",
        "network": "privatenet"
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
                        "identifier": "0xd79d229fe054406adc6d7441d0b605d37d00c1c72272a2744db200f87c9ba2c1:0"
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
                }
            }
        ],
    "metadata": {
        "tx_type": "ContractTransaction"
    }
}

response:
{
    "options": {
        "tx_type": "ContractTransaction"
    }
}

## /construction/metadata
request:
{
    "network_identifier": {
        "blockchain": "neo",
        "network": "privatenet"
    },
    "options": {
        "tx_type": "ContractTransaction"
    }
}

response:
{
    "metadata": {
        "tx_type": "ContractTransaction"
    }
}

## /construction/payloads
request:
{
    "network_identifier": {
        "blockchain": "neo",
        "network": "privatenet"
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
                        "identifier": "0xd79d229fe054406adc6d7441d0b605d37d00c1c72272a2744db200f87c9ba2c1:0"
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
                }
            }
        ],
    "metadata": {
        "tx_type": "ContractTransaction"
    }
}

response:
{
    "unsigned_transaction": "80000001c1a29b7cf800b24d74a27222c7c1007dd305b6d041746ddc6a4054e09f229dd70000019b7cffdaa674beae0f930ebe6085af9093e5fe56b34a5c220ccdcf6efc336fc50000c16ff2862300651897dcd926ab80a2026eaf7f1aa451361d60d2",
    "payloads": [
        {
            "address": "AQzRMe3zyGS8W177xLJfewRRQZY2kddMun",
            "hex_bytes": "80000001c1a29b7cf800b24d74a27222c7c1007dd305b6d041746ddc6a4054e09f229dd70000019b7cffdaa674beae0f930ebe6085af9093e5fe56b34a5c220ccdcf6efc336fc50000c16ff2862300651897dcd926ab80a2026eaf7f1aa451361d60d2",
            "signature_type": "ecdsa"
        }
    ]
}

## /construction/parse
request:
{
    "network_identifier": {
        "blockchain": "neo",
        "network": "privatenet"
    },
    "signed": false,
    "transaction": "80000001c1a29b7cf800b24d74a27222c7c1007dd305b6d041746ddc6a4054e09f229dd70000019b7cffdaa674beae0f930ebe6085af9093e5fe56b34a5c220ccdcf6efc336fc50000c16ff2862300651897dcd926ab80a2026eaf7f1aa451361d60d2"
}

response:
{
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
                    "identifier": "0xd79d229fe054406adc6d7441d0b605d37d00c1c72272a2744db200f87c9ba2c1:0"
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
                    "identifier": "0xedd732ab078e82e7fcaa729a3f778b4045a3cb1d9e3ea659b2943e8b7c8d9115:0"
                },
                "coin_action": "coin_created"
            }
        }
    ],
    "signers": [],
    "metadata": {
        "tx_type": "ContractTransaction"
    }
}

## /construction/combine
request:
{
    "network_identifier": {
        "blockchain": "neo",
        "network": "privatenet"
    },
    "unsigned_transaction": "80000001c1a29b7cf800b24d74a27222c7c1007dd305b6d041746ddc6a4054e09f229dd70000019b7cffdaa674beae0f930ebe6085af9093e5fe56b34a5c220ccdcf6efc336fc50000c16ff2862300651897dcd926ab80a2026eaf7f1aa451361d60d2",
    "signatures": [
        {
            "signing_payload": {
                "address": "AQzRMe3zyGS8W177xLJfewRRQZY2kddMun",
                "hex_bytes": "80000001c1a29b7cf800b24d74a27222c7c1007dd305b6d041746ddc6a4054e09f229dd70000019b7cffdaa674beae0f930ebe6085af9093e5fe56b34a5c220ccdcf6efc336fc50000c16ff2862300651897dcd926ab80a2026eaf7f1aa451361d60d2",
                "signature_type": "ecdsa"
            },
            "public_key": {
                "hex_bytes": "03d08d6f766b54e35745bc99d643c939ec6f3d37004f2a59006be0e53610f0be25",
                "curve_type": "secp256r1"
            },
            "signature_type": "ecdsa",
            "hex_bytes": "ca2c38b1bb16fb8f598c6e9a921c37611b563d918874414cc0379950b9757fe559b55dc13c5cae338bbc2695af1d39fdd1a2ff1d6eca885a6c14e89236d401c2"
        }
    ]
}

response:
{
    "signed_transaction": "80000001c1a29b7cf800b24d74a27222c7c1007dd305b6d041746ddc6a4054e09f229dd70000019b7cffdaa674beae0f930ebe6085af9093e5fe56b34a5c220ccdcf6efc336fc50000c16ff2862300651897dcd926ab80a2026eaf7f1aa451361d60d2014140ca2c38b1bb16fb8f598c6e9a921c37611b563d918874414cc0379950b9757fe559b55dc13c5cae338bbc2695af1d39fdd1a2ff1d6eca885a6c14e89236d401c2232103d08d6f766b54e35745bc99d643c939ec6f3d37004f2a59006be0e53610f0be25ac"
}

## /construction/parse
request:
{
    "network_identifier": {
        "blockchain": "neo",
        "network": "privatenet"
    },
    "signed": true,
    "transaction": "80000001c1a29b7cf800b24d74a27222c7c1007dd305b6d041746ddc6a4054e09f229dd70000019b7cffdaa674beae0f930ebe6085af9093e5fe56b34a5c220ccdcf6efc336fc50000c16ff2862300651897dcd926ab80a2026eaf7f1aa451361d60d2014140ca2c38b1bb16fb8f598c6e9a921c37611b563d918874414cc0379950b9757fe559b55dc13c5cae338bbc2695af1d39fdd1a2ff1d6eca885a6c14e89236d401c2232103d08d6f766b54e35745bc99d643c939ec6f3d37004f2a59006be0e53610f0be25ac"
}

response:
{
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
                    "identifier": "0xd79d229fe054406adc6d7441d0b605d37d00c1c72272a2744db200f87c9ba2c1:0"
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
                    "identifier": "0xedd732ab078e82e7fcaa729a3f778b4045a3cb1d9e3ea659b2943e8b7c8d9115:0"
                },
                "coin_action": "coin_created"
            }
        }
    ],
    "signers": [
        "AQzRMe3zyGS8W177xLJfewRRQZY2kddMun"
    ],
    "metadata": {
        "tx_type": "ContractTransaction"
    }
}

## /construction/hash
request:
{
    "network_identifier": {
        "blockchain": "neo",
        "network": "privatenet"
    },
    "signed_transaction": "80000001f82fc2c5bbc997bb6508d79c6fe6741737dd1073ae3fa10f5ab6c7a2da4a7d2e0000019b7cffdaa674beae0f930ebe6085af9093e5fe56b34a5c220ccdcf6efc336fc50000c16ff2862300651897dcd926ab80a2026eaf7f1aa451361d60d2014140759c7529c26cad3ea6307fa460fb7468b1d01a37fdce2cb8931ea588b57f2a834361922931fddf306c0728965e027bab566648f48eb9346fa2405c7f4c87b271232103d08d6f766b54e35745bc99d643c939ec6f3d37004f2a59006be0e53610f0be25ac"
}

response:
{
    "transaction_hash": "0xedd732ab078e82e7fcaa729a3f778b4045a3cb1d9e3ea659b2943e8b7c8d9115"
}

## /construction/submit
request:
{
    "network_identifier": {
        "blockchain": "neo",
        "network": "privatenet"
    },
    "signed_transaction": "80000001f82fc2c5bbc997bb6508d79c6fe6741737dd1073ae3fa10f5ab6c7a2da4a7d2e0000019b7cffdaa674beae0f930ebe6085af9093e5fe56b34a5c220ccdcf6efc336fc50000c16ff2862300651897dcd926ab80a2026eaf7f1aa451361d60d2014140759c7529c26cad3ea6307fa460fb7468b1d01a37fdce2cb8931ea588b57f2a834361922931fddf306c0728965e027bab566648f48eb9346fa2405c7f4c87b271232103d08d6f766b54e35745bc99d643c939ec6f3d37004f2a59006be0e53610f0be25ac"
}

response:
{
    "transaction_identifier": {
        "hash": "0xedd732ab078e82e7fcaa729a3f778b4045a3cb1d9e3ea659b2943e8b7c8d9115"
    }
}
