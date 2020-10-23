# Neo Rosetta API Plugin | Construction API

## Overview

The construction APIs can be used to generate address from a public key, build a transaction and send the transaction to the blockchain. The available APIs are listed below.

## APIs

### Construction

#### Derive a neo address from a public key

Method: **POST**

URL: `/construction/derive`

Sample request:

```json
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
```

The `public_key` is compressed and in hex string format.

Sample response:

```json
{
    "address": "AQzRMe3zyGS8W177xLJfewRRQZY2kddMun"
}
```

The `address` is **base58** encoded.

#### Generate a metadata request

Method: **POST**

URL: `/construction/preprocess`

Sample request:

```json
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
```

Sample response:

```json
{
    "options": {
        "tx_type": "ContractTransaction"
    }
}
```

The `options` include transaction type and information related to that type. The output of this API will be part of the input of `/construction/metadata`.

#### Retrieve metadata needed for transaction construction

Method: **POST**

URL: `/construction/metadata`

Sample request:

```json
{
    "network_identifier": {
        "blockchain": "neo",
        "network": "privatenet"
    },
    "options": {
        "tx_type": "ContractTransaction"
    }
}
```

Sample response:

```json
{
    "metadata": {
        "tx_type": "ContractTransaction"
    }
}
```

#### Generate an unsigned transaction and related signing payloads

Method: **POST**

URL: `/construction/payloads`

Sample request:

```json
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
```

Sample response:

```json
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
```

#### Parse a signed/unsigned transaction

Method: **POST**

URL: `/construction/parse`

Sample request:

```json
{
    "network_identifier": {
        "blockchain": "neo",
        "network": "privatenet"
    },
    "signed": false,
    "transaction": "80000001c1a29b7cf800b24d74a27222c7c1007dd305b6d041746ddc6a4054e09f229dd70000019b7cffdaa674beae0f930ebe6085af9093e5fe56b34a5c220ccdcf6efc336fc50000c16ff2862300651897dcd926ab80a2026eaf7f1aa451361d60d2"
}
```

Sample response:

```json
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
```

#### Combine an unsigned transaction with its signing payloads

Method: **POST**

URL: `/construction/combine`

Sample request:

```json
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
```

Sample response:

```json
{
    "signed_transaction": "80000001c1a29b7cf800b24d74a27222c7c1007dd305b6d041746ddc6a4054e09f229dd70000019b7cffdaa674beae0f930ebe6085af9093e5fe56b34a5c220ccdcf6efc336fc50000c16ff2862300651897dcd926ab80a2026eaf7f1aa451361d60d2014140ca2c38b1bb16fb8f598c6e9a921c37611b563d918874414cc0379950b9757fe559b55dc13c5cae338bbc2695af1d39fdd1a2ff1d6eca885a6c14e89236d401c2232103d08d6f766b54e35745bc99d643c939ec6f3d37004f2a59006be0e53610f0be25ac"
}
```

#### Retrieve the hash of a signed transaction

Method: **POST**

URL: `/construction/hash`

Sample request:

```json
{
    "network_identifier": {
        "blockchain": "neo",
        "network": "privatenet"
    },
    "signed_transaction": "80000001f82fc2c5bbc997bb6508d79c6fe6741737dd1073ae3fa10f5ab6c7a2da4a7d2e0000019b7cffdaa674beae0f930ebe6085af9093e5fe56b34a5c220ccdcf6efc336fc50000c16ff2862300651897dcd926ab80a2026eaf7f1aa451361d60d2014140759c7529c26cad3ea6307fa460fb7468b1d01a37fdce2cb8931ea588b57f2a834361922931fddf306c0728965e027bab566648f48eb9346fa2405c7f4c87b271232103d08d6f766b54e35745bc99d643c939ec6f3d37004f2a59006be0e53610f0be25ac"
}
```

Sample response:

```json
{
    "transaction_hash": "0xedd732ab078e82e7fcaa729a3f778b4045a3cb1d9e3ea659b2943e8b7c8d9115"
}
```

#### Submit a signed transaction

Method: **POST**

URL: `/construction/submit`

Sample request:

```json
{
    "network_identifier": {
        "blockchain": "neo",
        "network": "privatenet"
    },
    "signed_transaction": "80000001f82fc2c5bbc997bb6508d79c6fe6741737dd1073ae3fa10f5ab6c7a2da4a7d2e0000019b7cffdaa674beae0f930ebe6085af9093e5fe56b34a5c220ccdcf6efc336fc50000c16ff2862300651897dcd926ab80a2026eaf7f1aa451361d60d2014140759c7529c26cad3ea6307fa460fb7468b1d01a37fdce2cb8931ea588b57f2a834361922931fddf306c0728965e027bab566648f48eb9346fa2405c7f4c87b271232103d08d6f766b54e35745bc99d643c939ec6f3d37004f2a59006be0e53610f0be25ac"
}
```

Sample response:

```json
{
    "transaction_identifier": {
        "hash": "0xedd732ab078e82e7fcaa729a3f778b4045a3cb1d9e3ea659b2943e8b7c8d9115"
    }
}
```
