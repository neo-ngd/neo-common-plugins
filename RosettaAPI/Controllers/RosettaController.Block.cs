using Microsoft.AspNetCore.Mvc;
using Neo.IO.Json;
using Neo.Ledger;
using Neo.Network.P2P.Payloads;
using Neo.Wallets;
using System;
using System.Collections.Generic;
using System.Linq;
using NeoBlock = Neo.Network.P2P.Payloads.Block;
using NeoTransaction = Neo.Network.P2P.Payloads.Transaction;

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
            var index = request.BlockIdentifier.Index;
            var hash = request.BlockIdentifier.Hash;
            if (index == null && hash == null)
                return Error.BLOCK_IDENTIFIER_INVALID.ToJson();
            if (index != null && index < 0)
                return Error.BLOCK_INDEX_INVALID.ToJson();

            NeoBlock neoBlock;
            UInt256 blockHash;
            if (hash != null)
            {
                if (!UInt256.TryParse(hash, out blockHash))
                    return Error.BLOCK_HASH_INVALID.ToJson();
            }
            else
            {
                blockHash = Blockchain.Singleton.GetBlockHash((uint)index);
                if (blockHash == null)
                    return Error.BLOCK_INDEX_INVALID.ToJson();
            }
            neoBlock = Blockchain.Singleton.GetBlock(blockHash);
            if (neoBlock == null)
                return Error.BLOCK_NOT_FOUND.ToJson();
            BlockIdentifier blockIdentifier = new BlockIdentifier(neoBlock.Index, neoBlock.Hash.ToString());

            // get parent block
            BlockIdentifier parentBlockIdentifier;
            if (neoBlock.Index == 0)
                parentBlockIdentifier = blockIdentifier;
            else
            {
                var parentBlockHash = Blockchain.Singleton.GetBlockHash(neoBlock.Index - 1);
                if (parentBlockHash == null)
                    return Error.BLOCK_INDEX_INVALID.ToJson();

                var parentBlock = Blockchain.Singleton.GetBlock(parentBlockHash);
                if (parentBlock == null)
                    return Error.BLOCK_NOT_FOUND.ToJson();

                parentBlockIdentifier = new BlockIdentifier(parentBlock.Index, parentBlock.Hash.ToString());
            }

            // handle transactions
            Transaction[] transactions = new Transaction[] { };
            TransactionIdentifier[] otherTransactions = new TransactionIdentifier[] { };
            foreach (var neoTx in neoBlock.Transactions)
            {
                var tx = ConvertTx(neoTx);
                if (tx == null)
                    continue;
                if (tx.Operations.Length > 0)
                    transactions = transactions.Append(tx).ToArray();
                else
                    otherTransactions = otherTransactions.Append(new TransactionIdentifier(neoTx.Hash.ToString())).ToArray();
            }

            Block block = new Block(blockIdentifier, parentBlockIdentifier, neoBlock.Timestamp * 1000, transactions);
            BlockResponse response = new BlockResponse(block, otherTransactions.Length > 0 ? otherTransactions : null);
            return response.ToJson();
        }

        /// <summary>
        /// Get a transaction in a block by its Transaction Identifier. 
        /// This endpoint should only be used when querying a node for a block does not return all transactions contained within it.
        /// Calling this endpoint requires reference to a BlockIdentifier because transaction parsing can change depending on which block contains the transaction
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JObject BlockTransaction(BlockTransactionRequest request)
        {
            var blockIndex = request.BlockIdentifier.Index;
            var blockHashString = request.BlockIdentifier.Hash;
            // check block
            if (!UInt256.TryParse(blockHashString, out UInt256 blockHash))
                return Error.BLOCK_HASH_INVALID.ToJson();
            var blockHash2 = Blockchain.Singleton.GetBlockHash((uint)blockIndex);
            if (!blockHash.Equals(blockHash2))
                return Error.BLOCK_HASH_INVALID.ToJson();
            var block = Blockchain.Singleton.GetBlock(blockHash);
            if (block == null)
                return Error.BLOCK_HASH_INVALID.ToJson();

            // check tx
            if (!UInt256.TryParse(request.TransactionIdentifier.Hash, out UInt256 txHash))
                return Error.TX_HASH_INVALID.ToJson();
            var tx = Blockchain.Singleton.GetTransaction(txHash);
            if (!block.Transactions.Contains(tx))
                return Error.TX_HASH_INVALID.ToJson();

            Transaction transaction = ConvertTx(tx);
            BlockTransactionResponse response = new BlockTransactionResponse(transaction);
            return response.ToJson();
        }

        /// <summary>
        /// This method converts a Neo tx to a Rosetta tx. Obsolete tx type will not be converted.
        /// Put utxo into operations. Put tx type and type specific info into metadata.
        /// </summary>
        /// <param name="neoTx"></param>
        /// <returns></returns>
        private Transaction ConvertTx(NeoTransaction neoTx)
        {
            Dictionary<string, JObject> metadata = new Dictionary<string, JObject>();
            string txType = "";
            switch (neoTx.Type)
            {
                case TransactionType.ClaimTransaction:
                    var ctx = neoTx as ClaimTransaction;
                    txType = nameof(ClaimTransaction);
                    metadata.Add("claims", new JArray(ctx.Claims.Select(p => p.ToJson())));
                    break;
                case TransactionType.ContractTransaction:
                    txType = nameof(ContractTransaction);
                    break;
                case TransactionType.InvocationTransaction:
                    var itx = neoTx as InvocationTransaction;
                    txType = nameof(InvocationTransaction);
                    metadata.Add("script", itx.Script.ToHexString());
                    metadata.Add("gas", itx.Gas.ToString());
                    break;
                case TransactionType.IssueTransaction:
                    txType = nameof(IssueTransaction);
                    break;
                case TransactionType.MinerTransaction:
                    var mtx = neoTx as MinerTransaction;
                    txType = nameof(MinerTransaction);
                    metadata.Add("nonce", mtx.Nonce.ToString());
                    break;
                case TransactionType.StateTransaction:
                    var stx = neoTx as StateTransaction;
                    txType = nameof(StateTransaction);
                    metadata.Add("descriptors", new JArray(stx.Descriptors.Select(p => p.ToJson())));
                    break;
                default:
                    return null;
            }
            metadata.Add("tx_type", txType);

            // handle utxo transfer
            Operation[] operations = HandleUtxoTransfer(neoTx);
            Transaction transaction = new Transaction(new TransactionIdentifier(neoTx.Hash.ToString()), operations, new Metadata(metadata));

            return transaction;
        }

        private Operation[] HandleUtxoTransfer(NeoTransaction tx)
        {
            var type = OperationType.Transfer;
            var status = OperationStatus.OPERATION_STATUS_SUCCESS.Status;
            // from operations
            Operation[] fromOperations = new Operation[] { };
            for (int i = 0; i < tx.Inputs.Length; i++)
            {
                var input = tx.Inputs[i];
                var prevTx = Blockchain.Singleton.GetTransaction(input.PrevHash);
                if (prevTx == null)
                    throw new Exception("previous tx not found");
                var prevOutput = prevTx.Outputs[input.PrevIndex];
                if (prevOutput == null)
                    throw new Exception("previous tx output not found");
                Operation operation = new Operation(new OperationIdentifier(i), type, status, null, 
                    new AccountIdentifier(prevOutput.ScriptHash.ToAddress()),
                    new Amount("-" + prevOutput.Value.ToString(), new Currency(prevOutput.AssetId)),
                    new CoinChange(new CoinIdentifier(input.PrevHash, input.PrevIndex), CoinAction.CoinSpent)
                );
                fromOperations = fromOperations.Append(operation).ToArray();
            }

            // to operations
            Operation[] toOperations = new Operation[] { };
            var fromCount = fromOperations.Length;
            OperationIdentifier[] relatedOperations = fromCount == 0 ? null : Enumerable.Range(0, fromCount).Select(p => new OperationIdentifier(p)).ToArray();
            //foreach (var output in tx.Outputs)
            for(int i = 0; i < tx.Outputs.Length; i++)
            {
                var output = tx.Outputs[i];
                Operation operation = new Operation(new OperationIdentifier(i + fromCount), type, status, relatedOperations, 
                    new AccountIdentifier(output.ScriptHash.ToAddress()),
                    new Amount(output.Value.ToString(), new Currency(output.AssetId)),
                    new CoinChange(new CoinIdentifier(tx.Hash, i), CoinAction.CoinCreated)
                    );
                toOperations = toOperations.Append(operation).ToArray();
            }

            return fromOperations.Concat(toOperations).ToArray();
        }
    }
}
