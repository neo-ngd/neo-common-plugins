using Neo.Cryptography.MPTTrie;
using Neo.IO;
using Neo.IO.Json;
using Neo.Persistence;
using Neo.SmartContract;
using System;
using System.Collections.Generic;
using System.Buffers.Binary;
using static Neo.Helper;

namespace Neo.Plugins
{
    internal partial class RosettaController
    {
        private const byte BlockPrefix = 0x00;
        private const byte TransactionPrefix = 0x01;
        private const byte StateRootPrefix = 0x02;

        private byte[] BlockKey(UInt256 hash)
        {
            return Concat(new byte[] { BlockPrefix }, hash.ToArray());
        }

        private byte[] TransactionKey(UInt256 hash)
        {
            return Concat(new byte[] { TransactionPrefix }, hash.ToArray());
        }

        public void SaveBlockJson(UInt256 blockHash, JObject json)
        {
            db.Put(BlockKey(blockHash), Utility.StrictUTF8.GetBytes(json.ToString()));
        }

        public void SaveTransactionJson(UInt256 txHash, JObject json)
        {
            db.Put(TransactionKey(txHash), Utility.StrictUTF8.GetBytes(json.ToString()));
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
