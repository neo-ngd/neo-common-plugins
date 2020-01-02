using Neo.IO;
using Neo.IO.Json;
using Neo.Ledger;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Plugins
{
    partial class GraphService: IGraphService
    {
        private readonly NeoSystem system;

        public GraphService(NeoSystem system)
        {
            this.system = system;
        }

        public string GetBestBlockHash()
        {
            return Blockchain.Singleton.CurrentBlockHash.ToString();
        }

        public JObject GetBlock(JObject key, bool verbose)
        {
            Block block;
            if (key is JNumber)
            {
                uint index = uint.Parse(key.AsString());
                block = Blockchain.Singleton.GetBlock(index);
            }
            else
            {
                UInt256 hash = UInt256.Parse(key.AsString());
                block = Blockchain.Singleton.View.GetBlock(hash);
            }
            if (block == null)
                throw new GraphException(-100, "Unknown block");
            if (verbose)
            {
                JObject json = block.ToJson();
                json["confirmations"] = Blockchain.Singleton.Height - block.Index + 1;
                UInt256 hash = Blockchain.Singleton.GetNextBlockHash(block.Hash);
                if (hash != null)
                    json["nextblockhash"] = hash.ToString();
                return json;
            }
            return block.ToArray().ToHexString();
        }

        public uint GetBlockCount()
        {
            return Blockchain.Singleton.Height + 1;
        }

        public string GetBlockHash(uint height)
        {
            if (height <= Blockchain.Singleton.Height)
            {
                return Blockchain.Singleton.GetBlockHash(height).ToString();
            }
            throw new GraphException(-100, "Invalid Height");
        }

        public JObject GetBlockHeader(JObject key, bool verbose)
        {
            Header header;
            if (key is JNumber)
            {
                uint height = uint.Parse(key.AsString());
                header = Blockchain.Singleton.GetHeader(height);
            }
            else
            {
                UInt256 hash = UInt256.Parse(key.AsString());
                header = Blockchain.Singleton.View.GetHeader(hash);
            }
            if (header == null)
                throw new GraphException(-100, "Unknown block");

            if (verbose)
            {
                JObject json = header.ToJson();
                json["confirmations"] = Blockchain.Singleton.Height - header.Index + 1;
                UInt256 hash = Blockchain.Singleton.GetNextBlockHash(header.Hash);
                if (hash != null)
                    json["nextblockhash"] = hash.ToString();
                return json;
            }

            return header.ToArray().ToHexString();
        }

        public string GetBlockSysFee(uint height)
        {
            if (height <= Blockchain.Singleton.Height)
                using (ApplicationEngine engine = NativeContract.GAS.TestCall("getSysFeeAmount", height))
                {
                    return engine.ResultStack.Peek().GetBigInteger().ToString();
                }
            throw new GraphException(-100, "Invalid Height");
        }

        public JObject GetContractState(UInt160 script_hash)
        {
            ContractState contract = Blockchain.Singleton.View.Contracts.TryGet(script_hash);
            return contract?.ToJson() ?? throw new GraphException(-100, "Unknown contract");
        }

        public JObject GetRawMemPool(bool shouldGetUnverified)
        {
            if (!shouldGetUnverified)
                return new JArray(Blockchain.Singleton.MemPool.GetVerifiedTransactions().Select(p => (JObject)p.Hash.ToString()));

            JObject json = new JObject();
            json["height"] = Blockchain.Singleton.Height;
            Blockchain.Singleton.MemPool.GetVerifiedAndUnverifiedTransactions(
                out IEnumerable<Transaction> verifiedTransactions,
                out IEnumerable<Transaction> unverifiedTransactions);
            json["verified"] = new JArray(verifiedTransactions.Select(p => (JObject)p.Hash.ToString()));
            json["unverified"] = new JArray(unverifiedTransactions.Select(p => (JObject)p.Hash.ToString()));
            return json;
        }

        public JObject GetRawTransaction(UInt256 hash, bool verbose)
        {
            Transaction tx = Blockchain.Singleton.GetTransaction(hash);
            if (tx == null)
                throw new GraphException(-100, "Unknown transaction");
            if (verbose)
            {
                JObject json = tx.ToJson();
                TransactionState txState = Blockchain.Singleton.View.Transactions.TryGet(hash);
                if (txState != null)
                {
                    Header header = Blockchain.Singleton.GetHeader(txState.BlockIndex);
                    json["blockhash"] = header.Hash.ToString();
                    json["confirmations"] = Blockchain.Singleton.Height - header.Index + 1;
                    json["blocktime"] = header.Timestamp;
                    json["vmState"] = txState.VMState;
                }
                return json;
            }
            return tx.ToArray().ToHexString();
        }

        public JObject GetStorage(UInt160 script_hash, byte[] key)
        {
            StorageItem item = Blockchain.Singleton.View.Storages.TryGet(new StorageKey
            {
                ScriptHash = script_hash,
                Key = key
            }) ?? new StorageItem();
            return item.Value?.ToHexString();
        }

        public uint GetTransactionHeight(UInt256 hash)
        {
            uint? height = Blockchain.Singleton.View.Transactions.TryGet(hash)?.BlockIndex;
            if (height.HasValue) return height.Value;
            throw new GraphException(-100, "Unknown transaction");
        }

        public JObject GetValidators()
        {
            using SnapshotView snapshot = Blockchain.Singleton.GetSnapshot();
            var validators = NativeContract.NEO.GetValidators(snapshot);
            return NativeContract.NEO.GetRegisteredValidators(snapshot).Select(p =>
            {
                JObject validator = new JObject();
                validator["publickey"] = p.PublicKey.ToString();
                validator["votes"] = p.Votes.ToString();
                validator["active"] = validators.Contains(p.PublicKey);
                return validator;
            }).ToArray();
        }
    }
}
