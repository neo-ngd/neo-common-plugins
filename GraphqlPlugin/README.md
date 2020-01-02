# GraphQL Guidelines
## Enabling Services

GraphQL is a query language for APIs and a runtime for fulfilling those queries with your existing data. GraphQL provides a complete and understandable description of the data in your API,gives clients the power to ask for exactly what they need and nothing more, makes it easier to evolve APIs over time, and enables powerful developer tools. Each neo-cli can optionally install GraphQL plugin to enable related services. You can type the following command to install the graphql plugin:

`install GraphQLPlugin`

After installation, you need to restart the neo-cli server for the plugin to take effect.

## Modifying configuration file
Before installing the plugin, you can modify the BindAddress, Port and other parameters in the config.json file in the GraphQL folder:

```
{
    "PluginConfiguration": {
    "BindAddress": "127.0.0.1",
    "Port": 20336,
    "SslCert": "",
    "SslCertPassword": "",
    "TrustedAuthorities": [],
    "RpcUser": "",
    "RpcPass": "",
    "MaxGasInvoke": 10,
    "MaxFee": 0.1,
    "DisabledMethods": [
      "openwallet"
    ]
  }
}
```

## Interface List
 
| Name | Param | Desc |  Remark |
|-------|-----|----|------|
| txbroadcasting | hex | Broadcast a transaction over the network ||
| blockrelaying | hex | Relay a new block to the network |Needs to be a consensus node|
| bestblockhash | - | Gets the hash of the tallest block in the main chain||
|block|index / hash| Returns the block information with the specified index or hash ||
|blockcount|-| Get the block count of the blockchain||
|blocksysfee|index|Get the system fees before the block with the specified index||
|transaction|txid| Get a transaction with the specified hash||
|transactionheight|txid|  Get the block index in which the transaction is found||
|contract|scripthash|Get a contract with the specified script hash||
|validators|-|Get latest validators||
|peers|-| Gets a list of nodes that are currently connected/disconnected by this node ||
|version|-| Get version of the connected node||
|connectioncount|-|  Get the current number of connections for the node||
|plugins|-| Get plugins loaded by the node||
|addressverification|address| Verify whether the address is a correct NEO address||
|scriptinvocation|script & [scripthashes]| Run a script through the virtual machine and get the result||
|functioninvocation|scripthash & operation & [params]| Invoke a smart contract with specified script hash, passing in an operation and the corresponding params||
|getstorage| scripthash & key| Get the stored value with the contract script hash and the key||

Note：`[]` means the parameter is optional.

## Test Tools
Graphql provides a postman-like tool UI-Playground, through which you can invoke the interface with ease. You can enter http://somewebsite.com:port/ui/playground in the browser to access it. The UI screenshot is as follows:

<img width="744" alt="ui" src="https://user-images.githubusercontent.com/12050350/71613797-485f5800-2be3-11ea-8765-74e98c27b17b.png">

The section `1` is the query part. You can enter the query name as well as the required fields, and click the execute button to view the results in the section `2` . The DOCS section `3` lists all the available interfaces provided by the server, as well as the parameters and return value types of the interface. The SCHEMA section `4` lists all the schema information defined by the server. In combination with these two parts, the client can customize the fields to be retrieved.

<img width="398" alt="doc-details" src="https://user-images.githubusercontent.com/12050350/71613765-2665d580-2be3-11ea-989f-b7511da7a066.png">

<img width="330" alt="schema-details" src="https://user-images.githubusercontent.com/12050350/71613784-3b426900-2be3-11ea-95b2-48ade71c2890.png">

## Query Example

To query a block with the specified hash, you can query the `block` interface and list the required fields as follows:

```
query{
  block(hash:"0xe50a2fcc8c48abedc2f2622fe53c97e615fa671eb62d7e8590adb91fcc4931ff"){
    hash,
    size,
    version,
    previousBlockHash,
    merkleRoot,
    timestamp,
    index,
    nextConsensus,
    witness
        {
        invocation,
        verification
        },
    
    consensusData {
        primaryIndex,
        nonce
    },
    transactions
        {
        hash,
        size,
        version,
        nonce,
        sender,
        systemFee,
        networkFee,
        validUntilBlock,	
        attributes{	
            data
        },
        cosigners{
            account,
            scopes
        },
        script,
        witnesses
            {
            invocation,
            verification
            }
        },
    confirmations
    }
}	

```
The result is as follows:

```
{
  "data": {
    "block": {
      "hash": "0x7d581e115ebe1c512eef985fd52d75336acb77826dafacc3281399a0e6204958",
      "size": 171,
      "version": 0,
      "previousBlockHash": "0x0000000000000000000000000000000000000000000000000000000000000000",
      "merkleRoot": "0x2356554875add37e5d3fb596af5e3727e617f45deefa9c475c61094b788de122",
      "timestamp": 1468595301000,
      "index": 0,
      "nextConsensus": "AHXWU3WYptFD6cGnE12pVBjnGxas8ADNuC",
      "witness": {
        "invocation": "",
        "verification": "UQ=="
      },
      "consensusData": {
        "primaryIndex": 0,
        "nonce": "000000007c2bac1d"
      },
      "transactions": [
        {
          "hash": "0x2b03f7a8db3649c9e2cb6d429dd358819b3fd536825d2a698e19de237583e60a",
          "size": 57,
          "version": 0,
          "nonce": 0,
          "sender": "Abf2qMs1pzQb8kYk9RuxtUb9jtRKJVuBJt",
          "systemFee": "0",
          "networkFee": "0",
          "validUntilBlock": 0,
          "attributes": [],
          "cosigners": [],
          "script": "aBI+f+g=",
          "witnesses": [
            {
              "invocation": "",
              "verification": "UQ=="
            }
          ]
        }
      ],
      "confirmations": 1
    }
  }
}
```

