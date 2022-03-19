using Neo.IO;
using Neo.IO.Json;
using Neo.SmartContract.Native;
using System;
using System.Linq;
using NeoBlock = Neo.Network.P2P.Payloads.Block;

namespace Neo.Plugins
{
    partial class RosettaController
    {
        /// <summary>
        /// Get a block by its Block Identifier. 
        /// If transactions are returned in the same call to the node as fetching the block, 
        /// the response should include these transactions in the Block object. 
        /// If not, an array of Transaction Identifiers should be returned.
        /// So /block/transaction fetches can be done to get all transaction information.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JObject Block(BlockRequest request)
        {
            if (request.NetworkIdentifier?.Blockchain?.ToLower() != "neo n3")
                return Error.NETWORK_IDENTIFIER_INVALID.ToJson();
            if (request.NetworkIdentifier?.Network?.ToLower() != network)
                return Error.NETWORK_IDENTIFIER_INVALID.ToJson();
            var snapshot = system.GetSnapshot();
            NeoBlock neoBlock;
            if (request.BlockIdentifier is null ||
                (request.BlockIdentifier.Index is null && request.BlockIdentifier.Hash is null))
            {
                neoBlock = NativeContract.Ledger.GetBlock(snapshot, NativeContract.Ledger.CurrentHash(snapshot));
            }
            else if (request.BlockIdentifier.Index is not null)
            {
                if (request.BlockIdentifier.Index < 0)
                    return Error.BLOCK_INDEX_INVALID.ToJson();
                neoBlock = NativeContract.Ledger.GetBlock(snapshot, (uint)request.BlockIdentifier.Index);
                if (neoBlock is null)
                    return Error.BLOCK_NOT_FOUND.ToJson();
                if (request.BlockIdentifier.Hash is not null &&
                    request.BlockIdentifier.Hash != neoBlock.Hash.ToString())
                    return Error.BLOCK_IDENTIFIER_INVALID.ToJson();
            }
            else
            {
                if (UInt256.TryParse(request.BlockIdentifier.Hash, out var hash))
                    return Error.BLOCK_HASH_INVALID.ToJson();
                neoBlock = NativeContract.Ledger.GetBlock(snapshot, hash);
                if (neoBlock is null)
                    return Error.BLOCK_NOT_FOUND.ToJson();

            }
            BlockIdentifier respIdentifier = new(neoBlock.Index, neoBlock.Hash.ToString());
            var raw = db.TryGet(BlockKey(neoBlock.Hash));
            if (raw is null)
                return Error.BLOCK_NOT_FOUND.ToJson();
            Block block = Plugins.Block.FromJson(JObject.Parse(raw));
            var transactions = ((JArray)block.Metadata["Transactions"]).Select(p => p.GetString()).ToArray();
            block.Transactions = new Transaction[transactions.Length];
            for (int i = 0; i < transactions.Length; i++)
                block.Transactions[i] = Transaction.FromJson(JObject.Parse(db.TryGet(TransactionKey(UInt256.Parse(transactions[i])))));
            block.Metadata = null;
            BlockResponse response = new(block, null);
            return response.ToJson();
        }

        /// <summary>
        /// Get a transaction in a block by its Transaction Identifier. 
        /// This endpoint should only be used when querying a node for a block does not return all transactions contained within it. 
        /// All transactions returned by this endpoint must be appended to any transactions returned by the /block method by consumers of this data. 
        /// Fetching a transaction by hash is considered an Explorer Method (which is classified under the Future Work section). 
        /// This method can be used to let consumers to paginate results when the block trasactions count is too big to be returned in a single BlockResponse. 
        /// Calling this endpoint requires reference to a BlockIdentifier because transaction parsing can change depending on which block contains the transaction. 
        /// For example, in Bitcoin it is necessary to know which block contains a transaction to determine the destination of fee payments. 
        /// Without specifying a block identifier, the node would have to infer which block to use (which could change during a re-org). 
        /// Implementations that require fetching previous transactions to populate the response (ex: Previous UTXOs in Bitcoin) may find it useful to run a cache within the Rosetta server in the /data directory (on a path that does not conflict with the node).
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JObject BlockTransaction(BlockTransactionRequest request)
        {
            if (request.NetworkIdentifier?.Blockchain?.ToLower() != "neo n3")
                return Error.NETWORK_IDENTIFIER_INVALID.ToJson();
            if (request.NetworkIdentifier?.Network?.ToLower() != network)
                return Error.NETWORK_IDENTIFIER_INVALID.ToJson();
          
            var blockIndex = request.BlockIdentifier.Index;
            var blockHashString = request.BlockIdentifier.Hash;
            var snapshot = system.GetSnapshot();

            if (!UInt256.TryParse(blockHashString, out UInt256 blockHash))
                return Error.BLOCK_HASH_INVALID.ToJson();
            var blockHash2 = NativeContract.Ledger.GetBlockHash(snapshot, (uint)blockIndex);
            if (!blockHash.Equals(blockHash2))
                return Error.BLOCK_HASH_INVALID.ToJson();
            var block = NativeContract.Ledger.GetBlock(snapshot, blockHash);
            if (block == null)
                return Error.BLOCK_NOT_FOUND.ToJson();

            // check tx
            if (!UInt256.TryParse(request.TransactionIdentifier.Hash, out UInt256 txHash))
                return Error.TX_HASH_INVALID.ToJson();
            var raw = db.TryGet(TransactionKey(txHash));
            if (raw is null)
                return Error.TX_NOT_FOUND.ToJson();
            if (txHash != block.Hash && !block.Transactions.Select(p => p.Hash).Contains(txHash))
                return Error.TX_NOT_FOUND.ToJson();
            Transaction transaction = Transaction.FromJson(JObject.Parse(raw));
            BlockTransactionResponse response = new(transaction);
            return response.ToJson();
        }
    }
}
