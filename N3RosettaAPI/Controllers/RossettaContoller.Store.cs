using Neo.Cryptography.MPTTrie;
using Neo.IO;
using Neo.IO.Json;
using Neo.Ledger;
using Neo.Persistence;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.Wallets;
using System;
using System.Collections.Generic;
using System.Buffers.Binary;
using System.Linq;
using static Neo.Helper;
using NeoBlock = Neo.Network.P2P.Payloads.Block;
using NeoTransaction = Neo.Network.P2P.Payloads.Transaction;
using Neo.VM;

namespace Neo.Plugins
{
    internal partial class RosettaController
    {
        private const byte BlockPrefix = 0x00;
        private const byte TransactionPrefix = 0x01;
        private const byte StateRootPrefix = 0x02;
        private readonly Dictionary<string, Currency> _tokens = new()
        {
            { NativeContract.NEO.Hash.ToString(), Currency.NEO },
            { NativeContract.GAS.Hash.ToString(), Currency.GAS }
        };

        private byte[] BlockKey(UInt256 hash)
        {
            return Concat(new byte[] { BlockPrefix }, hash.ToArray());
        }

        private byte[] TransactionKey(UInt256 hash)
        {
            return Concat(new byte[] { TransactionPrefix }, hash.ToArray());
        }

        /// <summary>
        /// Convert native contracts token mint as a transaction which has the same
        /// hash with the block.
        /// </summary>
        /// <param name="blockHash"></param>
        /// <returns></returns>
        private Transaction SystemMintTransaction(UInt256 blockHash, List<Blockchain.ApplicationExecuted> execResult)
        {
            int index = 0;
            List<Operation> operations = new();
            foreach (var execution in execResult)
            {
                foreach (var notification in execution.Notifications)
                {
                    if (!_tokens.TryGetValue(notification.ScriptHash?.ToString(), out var currency))
                        continue;
                    if (notification.EventName != "Transfer")
                        continue;
                    var states = notification.State;
                    if (states[0].Type != VM.Types.StackItemType.Any)
                        continue;
                    if (states[1].Type != VM.Types.StackItemType.ByteString)
                        continue;
                    var toBytes = states[1].GetSpan();
                    if (toBytes.Length != UInt160.Length) continue;
                    var to = new UInt160(toBytes);
                    var amount = states[2].GetInteger().ToString();
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

        public void SaveBlock(NeoBlock neoBlock, List<Blockchain.ApplicationExecuted> execResult)
        {
            BlockIdentifier identifier = new(neoBlock.Index, neoBlock.Hash.ToString());
            BlockIdentifier parent = identifier;
            if (neoBlock.Index > 0)
                parent = new(neoBlock.Index - 1, neoBlock.PrevHash.ToString());
            Metadata meta = new(new Dictionary<string, JObject>{
                {"Transactions", new JArray(new JObject[]{(JObject)neoBlock.Hash.ToString()}.Concat(neoBlock.Transactions.Select(p => (JObject)p.Hash.ToString())))}
            });
            Block block = new(identifier, parent, (long)neoBlock.Timestamp, Array.Empty<Transaction>(), meta);
            db.Put(BlockKey(neoBlock.Hash), block.ToJson().ToByteArray(false));
            Transaction systemTx = SystemMintTransaction(neoBlock.Hash, execResult);
            db.Put(TransactionKey(neoBlock.Hash), systemTx.ToJson().ToByteArray(false));
        }

        /// <summary>
        /// This method converts a TransactionState to a Rosetta tx.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private Transaction ConvertTx(NeoBlock block, NeoTransaction neoTx, Blockchain.ApplicationExecuted execResult)
        {
            Dictionary<string, JObject> metadata = new();

            // get Operation[]
            Operation[] operations = GetOperations(neoTx, execResult);

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

        private Operation[] GetOperations(NeoTransaction neoTx, Blockchain.ApplicationExecuted execResult)
        {
            int index = 0;
            List<Operation> operations = new(){
                new(new(index++),
                    OperationType.Transfer,
                    OperationStatus.OPERATION_STATUS_SUCCESS.Status,
                    null,
                    new(neoTx.Sender.ToAddress(system.Settings.AddressVersion)),
                    new Amount($"-{neoTx.NetworkFee + neoTx.SystemFee}", Currency.GAS),
                    null)
            };
            if (execResult.VMState != VM.VMState.HALT) return operations.ToArray();
            foreach (var notification in execResult.Notifications)
            {
                if (notification.EventName == "Transfer")
                {
                    var tokenHashString = notification.ScriptHash.ToString();
                    if (!_tokens.TryGetValue(tokenHashString, out var currency))
                    {
                        continue;
                    }
                    var states = notification.State;
                    UInt160 from = null, to = null;
                    if (states[0].Type != VM.Types.StackItemType.Any)
                    {
                        var fromBytes = states[0].GetSpan();
                        if (fromBytes.Length == UInt160.Length)
                            from = new UInt160(fromBytes);
                    }
                    if (states[1].Type != VM.Types.StackItemType.Any)
                    {
                        var toBytes = states[1].GetSpan();
                        if (toBytes.Length == UInt160.Length)
                            to = new UInt160(toBytes);
                    }
                    var amount = states[2].GetInteger().ToString();
                    if (from is not null)
                        operations.Add(new Operation(new OperationIdentifier(index++),
                            OperationType.Transfer,
                            OperationStatus.OPERATION_STATUS_SUCCESS.Status,
                            null,
                            new AccountIdentifier(from.ToAddress(system.Settings.AddressVersion)),
                            new Amount("-" + amount, currency),
                            null
                            ));
                    if (to is not null)
                        operations.Add(new Operation(new OperationIdentifier(index++),
                            OperationType.Transfer,
                            OperationStatus.OPERATION_STATUS_SUCCESS.Status,
                            from is null ? null : new OperationIdentifier[] { new OperationIdentifier(index - 2) },
                            new AccountIdentifier(to.ToAddress(system.Settings.AddressVersion)),
                            new Amount(amount, currency),
                            null
                            ));
                }
            }
            return operations.ToArray();
        }

        public void SaveTransaction(NeoBlock block, NeoTransaction neoTx, Blockchain.ApplicationExecuted execResult)
        {
            db.Put(TransactionKey(neoTx.Hash), ConvertTx(block, neoTx, execResult).ToJson().ToByteArray(false));
        }

        private byte[] StateRootKey(uint index)
        {
            byte[] buffer = new byte[sizeof(uint) + 1];
            buffer[0] = StateRootPrefix;
            BinaryPrimitives.WriteUInt32BigEndian(buffer.AsSpan(1), index);
            return buffer;
        }

        private UInt256 GetStateRoot(uint index)
        {
            var raw = db.TryGet(StateRootKey(index));
            if (raw is null) return null;
            return new UInt256(raw);
        }

        public void SaveStates(uint height, List<DataCache.Trackable> change_set)
        {
            var snapshot = db.GetSnapshot();
            var trie = new Trie<StorageKey, StorageItem>(snapshot, height == 0 ? null : GetStateRoot(height - 1), true);
            foreach (var item in change_set)
            {
                switch (item.State)
                {
                    case TrackState.Added:
                        trie.Put(item.Key, item.Item);
                        break;
                    case TrackState.Changed:
                        trie.Put(item.Key, item.Item);
                        break;
                    case TrackState.Deleted:
                        trie.Delete(item.Key);
                        break;
                }
            }
            trie.Commit();
            UInt256 root_hash = trie.Root.Hash;
            snapshot.Put(StateRootKey(height), root_hash.ToArray());
            snapshot.Commit();
        }
    }
}
