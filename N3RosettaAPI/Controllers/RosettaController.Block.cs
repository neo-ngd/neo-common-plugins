﻿using Neo.IO;
using Neo.IO.Data.LevelDB;
using Neo.IO.Json;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.VM;
using Neo.Wallets;
using System;
using System.Collections.Generic;
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
            if (request.NetworkIdentifier.Blockchain.ToLower() != "neo n3")
                return Error.NETWORK_IDENTIFIER_INVALID.ToJson();

            var index = request.BlockIdentifier.Index;
            var hash = request.BlockIdentifier.Hash;
            var snapshot = system.GetSnapshot();

            NeoBlock neoBlock;
            if (index != null)
            {
                if (index < 0)
                    return Error.BLOCK_INDEX_INVALID.ToJson();
                else
                    neoBlock = NativeContract.Ledger.GetBlock(snapshot, (uint)index);
                // check if match the hash provided
                if (!string.IsNullOrEmpty(hash))
                {
                    var neoBlock2 = NativeContract.Ledger.GetBlock(snapshot, UInt256.Parse(hash));
                    if (neoBlock.Hash != neoBlock2.Hash)
                        return Error.BLOCK_IDENTIFIER_INVALID.ToJson();
                }
            }
            else if (!string.IsNullOrEmpty(hash))
                neoBlock = NativeContract.Ledger.GetBlock(snapshot, UInt256.Parse(hash));
            else
            {
                // get the current block
                var currentIndex = NativeContract.Ledger.CurrentIndex(snapshot);
                neoBlock = NativeContract.Ledger.GetBlock(snapshot, currentIndex);
            }

            if (neoBlock == null)
                return Error.BLOCK_NOT_FOUND.ToJson();
            BlockIdentifier blockIdentifier = new BlockIdentifier(neoBlock.Index, neoBlock.Hash.ToString());

            // get parent block
            BlockIdentifier parentBlockIdentifier;
            if (neoBlock.Index == 0)
                parentBlockIdentifier = blockIdentifier;
            else
            {
                var parentBlock = NativeContract.Ledger.GetBlock(snapshot, neoBlock.Index - 1);
                if (parentBlock == null)
                    return Error.BLOCK_NOT_FOUND.ToJson();

                parentBlockIdentifier = new BlockIdentifier(parentBlock.Index, parentBlock.Hash.ToString());
            }

            // handle transactions
            List<Transaction> transactions = new();
            List<TransactionIdentifier> otherTransactions = new();
            transactions.Add(SystemMintTransaction(neoBlock.Hash));
            foreach (var neoTx in neoBlock.Transactions)
            {
                TransactionState state = NativeContract.Ledger.GetTransactionState(system.StoreView, neoTx.Hash);
                if (state is null || state.Transaction is null)
                    return Error.TX_NOT_FOUND.ToJson();
                var tx = ConvertTx(state);
                if (tx == null)
                    continue;
                if (tx.Operations.Length > 0)
                    transactions.Add(tx);
                else
                    otherTransactions.Add(new TransactionIdentifier(neoTx.Hash.ToString()));
            }

            Block block = new(blockIdentifier, parentBlockIdentifier, (long)(neoBlock.Timestamp * 1000), transactions.ToArray());
            BlockResponse response = new(block, otherTransactions.Count > 0 ? otherTransactions.ToArray() : null);
            return response.ToJson();
        }

        /// <summary>
        /// Convert native contracts token mint as a transaction which has the same
        /// hash with the block.
        /// </summary>
        /// <param name="blockHash"></param>
        /// <returns></returns>
        private Transaction SystemMintTransaction(UInt256 blockHash)
        {
            int index = 0;
            List<Operation> operations = new();
            byte[] value = db.Get(ReadOptions.Default, blockHash.ToArray());
            if (value is null) return null;
            var json = JObject.Parse(Utility.StrictUTF8.GetString(value));
            foreach (var execution in (JArray)json["executions"])
            {
                foreach (var notification in (JArray)execution["notifications"])
                {
                    if (!_tokens.TryGetValue(notification["contract"]?.GetString(), out var currency))
                        continue;
                    if (notification["eventname"]?.GetString() != "Transfer")
                        continue;
                    var states = (JArray)notification["state"]["value"];
                    if (states[0]["type"]?.GetString() != "Any")
                        continue;
                    if (states[1]["type"]?.GetString() != "ByteString")
                        continue;
                    var toBase64 = states[1]["value"]?.GetString();
                    if (toBase64 is null) continue;
                    var to = new UInt160(Convert.FromBase64String(toBase64));
                    var amount = states[2]["value"]?.GetString();
                    if (amount is null) continue;
                    operations.Add(new Operation(new OperationIdentifier(index++),
                        OperationType.Transfer,
                        OperationStatus.OPERATION_STATUS_SUCCESS.Status,
                        null,
                        new AccountIdentifier(to.ToAddress(system.Settings.AddressVersion)),
                        new Amount(amount, currency),
                        null
                        ));
                }
            }
            return new(new(blockHash.ToString()), operations.ToArray(), null);
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
            if (request.NetworkIdentifier.Blockchain.ToLower() != "neo n3")
                return Error.NETWORK_IDENTIFIER_INVALID.ToJson();

            // check block
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
            if (txHash == block.Hash)
                return new BlockTransactionResponse(SystemMintTransaction(block.Hash)).ToJson();
            TransactionState state = NativeContract.Ledger.GetTransactionState(system.StoreView, txHash);
            if (state is null || state.Transaction is null)
                return Error.TX_NOT_FOUND.ToJson();
            if (!block.Transactions.Contains(state.Transaction))
                return Error.TX_NOT_FOUND.ToJson();

            Transaction transaction = ConvertTx(state);
            BlockTransactionResponse response = new BlockTransactionResponse(transaction);
            return response.ToJson();
        }

        /// <summary>
        /// This method converts a TransactionState to a Rosetta tx.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private Transaction ConvertTx(TransactionState state)
        {
            Dictionary<string, JObject> metadata = new Dictionary<string, JObject>();

            var neoTx = state.Transaction;

            // get Operation[]
            Operation[] operations = GetOperations(neoTx);

            TrimmedBlock block = NativeContract.Ledger.GetTrimmedBlock(system.StoreView, NativeContract.Ledger.GetBlockHash(system.StoreView, state.BlockIndex));

            metadata.Add("size", neoTx.Size);
            metadata.Add("version", neoTx.Version);
            metadata.Add("nonce", neoTx.Nonce);
            metadata.Add("sender", neoTx.Sender.ToAddress(system.Settings.AddressVersion));
            metadata.Add("sysfee", neoTx.SystemFee.ToString());
            metadata.Add("netfee", neoTx.NetworkFee.ToString());
            metadata.Add("validuntilblock", neoTx.ValidUntilBlock);
            metadata.Add("signers", neoTx.Signers.Select(p => p.ToJson()).ToArray());
            metadata.Add("attributes", neoTx.Attributes.Select(p => p.ToJson()).ToArray());
            metadata.Add("script", Convert.ToBase64String(neoTx.Script));
            metadata.Add("witnesses", neoTx.Witnesses.Select(p => p.ToJson()).ToArray());
            metadata.Add("blockhash", block.Hash.ToString());
            metadata.Add("blocktime", block.Header.Timestamp);

            return new Transaction(new TransactionIdentifier(neoTx.Hash.ToString()), operations, new Metadata(metadata));
        }

        private readonly Dictionary<string, Currency> _tokens = new()
        {
            { NativeContract.NEO.Hash.ToString(), Currency.NEO },
            { NativeContract.GAS.Hash.ToString(), Currency.GAS }
        };

        private Operation[] GetOperations(Network.P2P.Payloads.Transaction neoTx)
        {
            int index = 0; // Operation index, starting from 0
            List<Operation> operations = new();
            //Fee operation
            operations.Add(new(new(index++),
                OperationType.Transfer,
                OperationStatus.OPERATION_STATUS_SUCCESS.Status,
                null,
                new(neoTx.Sender.ToAddress(system.Settings.AddressVersion)),
                new Amount($"-{neoTx.NetworkFee + neoTx.SystemFee}", Currency.GAS),
                null));
            byte[] value = db.Get(ReadOptions.Default, neoTx.Hash.ToArray());
            if (!(value is null))
            {
                var raw = JObject.Parse(Neo.Utility.StrictUTF8.GetString(value));
                var executions = raw["executions"] as JArray;
                if (executions.Count > 0)
                {
                    var execution = executions[0];
                    var vmState = execution["vmstate"];
                    if (vmState.AsString() == "HALT")
                    {
                        var notifications = execution["notifications"] as JArray;
                        for (int i = 0; i < notifications.Count; i++)
                        {
                            var notification = notifications[i];
                            if (notification["eventname"].AsString() == "Transfer")
                            {
                                var tokenHashString = notification["contract"].AsString();
                                if (!_tokens.TryGetValue(tokenHashString, out var currency))
                                {
                                    continue;
                                }
                                var states = notification["state"]["value"] as JArray;
                                UInt160 from = null, to = null;
                                if (states[0]["type"]?.GetString() != "Any")
                                {
                                    var fromBase64 = states[0]["value"]?.GetString();
                                    if (fromBase64 is not null)
                                        from = new UInt160(Convert.FromBase64String(fromBase64));
                                }
                                if (states[1]["type"]?.GetString() != "Any")
                                {
                                    var toBase64 = states[1]["value"]?.GetString();
                                    if (toBase64 is not null)
                                        to = new UInt160(Convert.FromBase64String(toBase64));
                                }
                                var amount = states[2]["value"].GetString();
                                if (from is not null)
                                    operations.Add(new Operation(new OperationIdentifier(index++),
                                        OperationType.Transfer,
                                        OperationStatus.OPERATION_STATUS_SUCCESS.Status, // vm state is HALT, FAULT transfer is ignored
                                        null,
                                        new AccountIdentifier(from.ToAddress(system.Settings.AddressVersion)),
                                        new Amount("-" + amount, currency),
                                        null
                                        ));
                                if (to is not null)
                                    operations.Add(new Operation(new OperationIdentifier(index++),
                                        OperationType.Transfer,
                                        OperationStatus.OPERATION_STATUS_SUCCESS.Status,
                                        from is null ? null : new OperationIdentifier[] { new OperationIdentifier(index - 2) }, // related Operation is the fromOperation
                                        new AccountIdentifier(to.ToAddress(system.Settings.AddressVersion)),
                                        new Amount(amount, currency),
                                        null
                                        ));
                            }
                        }
                    }
                }
            }
            return operations.ToArray();
        }

        private (string, int) GetSymbolAndDecimals(UInt160 tokenHash)
        {
            byte[] script;
            using (ScriptBuilder sb = new())
            {
                sb.EmitDynamicCall(tokenHash, "symbol");
                sb.EmitDynamicCall(tokenHash, "decimals");
                script = sb.ToArray();
            }
            using ApplicationEngine engine = ApplicationEngine.Run(script, system.StoreView, settings: system.Settings);
            if (engine.State == VMState.HALT)
            {
                byte decimals = (byte)engine.ResultStack.Pop().GetInteger();
                string symbol = engine.ResultStack.Pop().GetString();
                return (symbol, decimals);
            }
            return (string.Empty, -1);
        }
    }
}
