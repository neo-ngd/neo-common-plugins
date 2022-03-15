using Neo.Cryptography.MPTTrie;
using Neo.Persistence;
using Neo.SmartContract;
using System.Collections.Generic;

namespace Neo.Plugins
{
    public class HistoricalDataCache : DataCache
    {
        private readonly Trie<StorageKey, StorageItem> trie;

        public HistoricalDataCache(IStore store, UInt256 root)
        {
            trie = new(store.GetSnapshot(), root, true);
        }

        protected override void AddInternal(StorageKey key, StorageItem value)
        {
            trie.Put(key, value.Clone());
        }

        protected override void DeleteInternal(StorageKey key)
        {
            trie.Delete(key);
        }

        protected override bool ContainsInternal(StorageKey key)
        {
            return trie.TryGetValue(key, out _);
        }

        protected override StorageItem GetInternal(StorageKey key)
        {
            return trie[key].Clone();
        }

        protected override IEnumerable<(StorageKey, StorageItem)> SeekInternal(byte[] keyOrPreifx, SeekDirection direction)
        {
            foreach (var (key, value) in trie.Find(keyOrPreifx))
                yield return (key, value.Clone());
        }

        protected override StorageItem TryGetInternal(StorageKey key)
        {
            if (trie.TryGetValue(key, out var item)) return item;
            return null;
        }

        protected override void UpdateInternal(StorageKey key, StorageItem value)
        {
            trie.Put(key, value);
        }
    }
}
