# Neo Rosetta API Plugin | Construction API

## Overview

The construction APIs can be used to generate address from a public key, build a transaction and send the transaction to the blockchain. 

### Flow of Operations

```
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
```

## APIs

### Construction

#### Derive a neo address from a public key

Method: **POST**

URL: `/construction/derive`

Sample request:

```json
{
    "network_identifier": {
        "blockchain": "neo n3",
        "network": "privatenet"
    },
    "public_key": {
        "hex_bytes": "02bc22b79376b4c5044a4f3aad56909ddf8403142929cc2d7dd459c2bd86181c28",
        "curve_type": "secp256r1"
    },
    "metadata": {}
}
```

The `public_key` is compressed and in hex string format.

Sample response:

```json
{
    "account_identifier": {
        "address": "NQdAHrGdt7QgMhNFUv5AuCYCKtu4cvTHeQ"
    }
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
        "blockchain": "neo n3",
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
                "address": "NfdK3KadbACwvHKZ5ygCsZa3MNgmucFUN6"
            },
            "amount": {
                "value": "-50000000000",
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
                "value": "50000000000",
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
        "signers": [
            {
                "account": "0x589e5b84d2e0e206c63a34415aa79064a1043cd8",
                "scopes": "CalledByEntry"
            }
        ]
    }
}
```

Sample response:

```json
{
    "options": {
        "signers": [
            {
                "account": "0x589e5b84d2e0e206c63a34415aa79064a1043cd8",
                "scopes": "CalledByEntry"
            }
        ],
        "has_existing_script": false,
        "operations_script": "DAdyb3NldHRhAwB0O6QLAAAADBQzqtPeyBur5jqI0A/wV3gYcQGDRwwU2DwEoWSQp1pBNDrGBuLg0oRbnlgUwB8MCHRyYW5zZmVyDBTPduKL0AYsSkeO41VhARMZ88+k0kFifVtS"
    }
}
```

In the request, caller can either pass `operations` or `script`. If both are passed, `script` will be added to the response. Alos, the `signers` MUST be passed.

In the response, the `options` includes the preprocessed metadata which will be part of the input of `/construction/metadata`.

#### Retrieve metadata needed for transaction construction

Method: **POST**

URL: `/construction/metadata`

Sample request:

```json
{
    "network_identifier": {
        "blockchain": "neo n3",
        "network": "privatenet"
    },
    "options": {
        "signers": [
            {
                "account": "0x589e5b84d2e0e206c63a34415aa79064a1043cd8",
                "scopes": "CalledByEntry"
            }
        ],
        "public_key_usages": [
            {
                "signer_account":"0x589e5b84d2e0e206c63a34415aa79064a1043cd8",
                "key_indexes":[0],
                "m":1
            }
        ],
        
        "has_existing_script": false,
        "operations_script": "DAdyb3NldHRhAwB0O6QLAAAADBQzqtPeyBur5jqI0A/wV3gYcQGDRwwU2DwEoWSQp1pBNDrGBuLg0oRbnlgUwB8MCHRyYW5zZmVyDBTPduKL0AYsSkeO41VhARMZ88+k0kFifVtS",
        "max_fee": 2000000000
    },
    "public_keys":[
        {
            "hex_bytes": "02bc22b79376b4c5044a4f3aad56909ddf8403142929cc2d7dd459c2bd86181c28",
            "curve_type": "secp256r1"
        }
    ]
}
```

Sample response:

```json
{
    "metadata": {
        "version": 0,
        "nonce": 741765923,
        "sysfee": 9977960,
        "netfee": 1243520,
        "validuntilblock": 8390,
        "script": "DAdyb3NldHRhAwB0O6QLAAAADBQzqtPeyBur5jqI0A/wV3gYcQGDRwwU2DwEoWSQp1pBNDrGBuLg0oRbnlgUwB8MCHRyYW5zZmVyDBTPduKL0AYsSkeO41VhARMZ88+k0kFifVtS",
        "signers": [
            {
                "account": "0x589e5b84d2e0e206c63a34415aa79064a1043cd8",
                "scopes": "CalledByEntry"
            }
        ]
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
        "blockchain": "neo n3",
        "network": "privatenet"
    },
    "operations":[],
    "metadata": {
        "version": 0,
        "nonce": 741765923,
        "sysfee": 9977960,
        "netfee": 1243520,
        "validuntilblock": 8390,
        "script": "DAdyb3NldHRhAwB0O6QLAAAADBQzqtPeyBur5jqI0A/wV3gYcQGDRwwU2DwEoWSQp1pBNDrGBuLg0oRbnlgUwB8MCHRyYW5zZmVyDBTPduKL0AYsSkeO41VhARMZ88+k0kFifVtS",
        "signers": [
            {
                "account": "0x589e5b84d2e0e206c63a34415aa79064a1043cd8",
                "scopes": "CalledByEntry"
            }
        ]
    }
}
```

Sample response:

```json
{
    "unsigned_transaction": "ACNzNixoQJgAAAAAAID5EgAAAAAAxiAAAAHYPAShZJCnWkE0OsYG4uDShFueWAEAZgwHcm9zZXR0YQMAdDukCwAAAAwUM6rT3sgbq+Y6iNAP8Fd4GHEBg0cMFNg8BKFkkKdaQTQ6xgbi4NKEW55YFMAfDAh0cmFuc2ZlcgwUz3bii9AGLEpHjuNVYQETGfPPpNJBYn1bUg==",
    "payloads": [
        {
            "account_identifier": {
                "address": "NfdK3KadbACwvHKZ5ygCsZa3MNgmucFUN6"
            },
            "hex_bytes": "002373362c684098000000000080f9120000000000c620000001d83c04a16490a75a41343ac606e2e0d2845b9e580100660c07726f73657474610300743ba40b0000000c1433aad3dec81babe63a88d00ff0577818710183470c14d83c04a16490a75a41343ac606e2e0d2845b9e5814c01f0c087472616e736665720c14cf76e28bd0062c4a478ee35561011319f3cfa4d241627d5b52",
            "signature_type": "ecdsa"
        }
    ]
}
```

#### Parse an unsigned transaction

Method: **POST**

URL: `/construction/parse`

Sample request:

```json
{
    "network_identifier": {
        "blockchain": "neo n3",
        "network": "privatenet"
    },
    "signed":false,
    "transaction": "ACNzNixoQJgAAAAAAID5EgAAAAAAxiAAAAHYPAShZJCnWkE0OsYG4uDShFueWAEAZgwHcm9zZXR0YQMAdDukCwAAAAwUM6rT3sgbq+Y6iNAP8Fd4GHEBg0cMFNg8BKFkkKdaQTQ6xgbi4NKEW55YFMAfDAh0cmFuc2ZlcgwUz3bii9AGLEpHjuNVYQETGfPPpNJBYn1bUg=="
}
```

Sample response:

```json
{
    "operations": [],
    "signers": [],
    "metadata": {
        "size": 152,
        "version": 0,
        "nonce": 741765923,
        "sysfee": 9977960,
        "netfee": 1243520,
        "sender": "NfdK3KadbACwvHKZ5ygCsZa3MNgmucFUN6",
        "script": "DAdyb3NldHRhAwB0O6QLAAAADBQzqtPeyBur5jqI0A/wV3gYcQGDRwwU2DwEoWSQp1pBNDrGBuLg0oRbnlgUwB8MCHRyYW5zZmVyDBTPduKL0AYsSkeO41VhARMZ88+k0kFifVtS",
        "signers": [
            {
                "account": "0x589e5b84d2e0e206c63a34415aa79064a1043cd8",
                "scopes": "CalledByEntry"
            }
        ],
        "attributes": [],
        "witnesses": []
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
        "blockchain": "neo n3",
        "network": "privatenet"
    },
    "unsigned_transaction":"ACNzNixoQJgAAAAAAID5EgAAAAAAxiAAAAHYPAShZJCnWkE0OsYG4uDShFueWAEAZgwHcm9zZXR0YQMAdDukCwAAAAwUM6rT3sgbq+Y6iNAP8Fd4GHEBg0cMFNg8BKFkkKdaQTQ6xgbi4NKEW55YFMAfDAh0cmFuc2ZlcgwUz3bii9AGLEpHjuNVYQETGfPPpNJBYn1bUg==",
    "signatures": [
        {
            "signing_payload": {
                "account_identifier": {
                    "address": "NfdK3KadbACwvHKZ5ygCsZa3MNgmucFUN6"
                },
                "hex_bytes": "002373362c684098000000000080f9120000000000c620000001d83c04a16490a75a41343ac606e2e0d2845b9e580100660c07726f73657474610300743ba40b0000000c1433aad3dec81babe63a88d00ff0577818710183470c14d83c04a16490a75a41343ac606e2e0d2845b9e5814c01f0c087472616e736665720c14cf76e28bd0062c4a478ee35561011319f3cfa4d241627d5b52",
                "signature_type": "ecdsa"
            },
            "public_key": {
                "hex_bytes": "0246ee1fef8677d3bc624256204db3223b0d0bbd960fd63efefd4c105c8828aa8c",
                "curve_type": "secp256r1"
            },
            "signature_type": "ecdsa",
            "hex_bytes": "0c2c286431efc7c818fec10335b3f97e37f928946aa19e8270725a10a27221e6a165365146c2574ed0733b6ae35cf73a43b0e217242e137a49473ead86b5ec53"
        }
    ],
    "signature_usages": [
         {
            "signer_account":"0x589e5b84d2e0e206c63a34415aa79064a1043cd8",
            "signature_indexes":[0],
            "m":1
        }
    ]
}
```

Sample response:

```json
{
    "signed_transaction": "ACNzNixoQJgAAAAAAID5EgAAAAAAxiAAAAHYPAShZJCnWkE0OsYG4uDShFueWAEAZgwHcm9zZXR0YQMAdDukCwAAAAwUM6rT3sgbq+Y6iNAP8Fd4GHEBg0cMFNg8BKFkkKdaQTQ6xgbi4NKEW55YFMAfDAh0cmFuc2ZlcgwUz3bii9AGLEpHjuNVYQETGfPPpNJBYn1bUgFCDEAMLChkMe/HyBj+wQM1s/l+N/kolGqhnoJwcloQonIh5qFlNlFGwldO0HM7auNc9zpDsOIXJC4TeklHPq2GtexTKAwhAkbuH++Gd9O8YkJWIE2zIjsNC72WD9Y+/v1MEFyIKKqMQVbnsyc="
}
```

#### Parse a signed transaction

Method: **POST**

URL: `/construction/parse`

Sample request:

```json
{
    "network_identifier": {
        "blockchain": "neo n3",
        "network": "privatenet"
    },
    "signed": true,
    "transaction": "ACNzNixoQJgAAAAAAID5EgAAAAAAxiAAAAHYPAShZJCnWkE0OsYG4uDShFueWAEAZgwHcm9zZXR0YQMAdDukCwAAAAwUM6rT3sgbq+Y6iNAP8Fd4GHEBg0cMFNg8BKFkkKdaQTQ6xgbi4NKEW55YFMAfDAh0cmFuc2ZlcgwUz3bii9AGLEpHjuNVYQETGfPPpNJBYn1bUgFCDEAMLChkMe/HyBj+wQM1s/l+N/kolGqhnoJwcloQonIh5qFlNlFGwldO0HM7auNc9zpDsOIXJC4TeklHPq2GtexTKAwhAkbuH++Gd9O8YkJWIE2zIjsNC72WD9Y+/v1MEFyIKKqMQVbnsyc="
}
```

Sample response:

```json
{
    "operations": [],
    "signers": [],
    "metadata": {
        "size": 260,
        "version": 0,
        "nonce": 741765923,
        "sysfee": 9977960,
        "netfee": 1243520,
        "sender": "NfdK3KadbACwvHKZ5ygCsZa3MNgmucFUN6",
        "script": "DAdyb3NldHRhAwB0O6QLAAAADBQzqtPeyBur5jqI0A/wV3gYcQGDRwwU2DwEoWSQp1pBNDrGBuLg0oRbnlgUwB8MCHRyYW5zZmVyDBTPduKL0AYsSkeO41VhARMZ88+k0kFifVtS",
        "signers": [
            {
                "account": "0x589e5b84d2e0e206c63a34415aa79064a1043cd8",
                "scopes": "CalledByEntry"
            }
        ],
        "attributes": [],
        "witnesses": [
            {
                "invocation": "DEAMLChkMe/HyBj+wQM1s/l+N/kolGqhnoJwcloQonIh5qFlNlFGwldO0HM7auNc9zpDsOIXJC4TeklHPq2GtexT",
                "verification": "DCECRu4f74Z307xiQlYgTbMiOw0LvZYP1j7+/UwQXIgoqoxBVuezJw=="
            }
        ]
    }
}
```

#### Retrieve the hash of a signed transaction

Method: **POST**

URL: `/construction/hash`

Sample request:

```json
{
    "network_identifier": {
        "blockchain": "neo n3",
        "network": "privatenet"
    },
    "signed_transaction": "ACNzNixoQJgAAAAAAID5EgAAAAAAxiAAAAHYPAShZJCnWkE0OsYG4uDShFueWAEAZgwHcm9zZXR0YQMAdDukCwAAAAwUM6rT3sgbq+Y6iNAP8Fd4GHEBg0cMFNg8BKFkkKdaQTQ6xgbi4NKEW55YFMAfDAh0cmFuc2ZlcgwUz3bii9AGLEpHjuNVYQETGfPPpNJBYn1bUgFCDEAMLChkMe/HyBj+wQM1s/l+N/kolGqhnoJwcloQonIh5qFlNlFGwldO0HM7auNc9zpDsOIXJC4TeklHPq2GtexTKAwhAkbuH++Gd9O8YkJWIE2zIjsNC72WD9Y+/v1MEFyIKKqMQVbnsyc="
}
```

Sample response:

```json
{
    "transaction_hash": "0x90ba86a97377e476dfbba39eb98d1b1315f3e52e9d14f77ff1451d9f40c1855d"
}
```

#### Submit a signed transaction

Method: **POST**

URL: `/construction/submit`

Sample request:

```json
{
    "network_identifier": {
        "blockchain": "neo n3",
        "network": "privatenet"
    },
    "signed_transaction": "ACNzNixoQJgAAAAAAID5EgAAAAAAxiAAAAHYPAShZJCnWkE0OsYG4uDShFueWAEAZgwHcm9zZXR0YQMAdDukCwAAAAwUM6rT3sgbq+Y6iNAP8Fd4GHEBg0cMFNg8BKFkkKdaQTQ6xgbi4NKEW55YFMAfDAh0cmFuc2ZlcgwUz3bii9AGLEpHjuNVYQETGfPPpNJBYn1bUgFCDEAMLChkMe/HyBj+wQM1s/l+N/kolGqhnoJwcloQonIh5qFlNlFGwldO0HM7auNc9zpDsOIXJC4TeklHPq2GtexTKAwhAkbuH++Gd9O8YkJWIE2zIjsNC72WD9Y+/v1MEFyIKKqMQVbnsyc="
}
```

Sample response:

```json
{
    "transaction_identifier": {
        "hash": "0x90ba86a97377e476dfbba39eb98d1b1315f3e52e9d14f77ff1451d9f40c1855d"
    }
}
```
