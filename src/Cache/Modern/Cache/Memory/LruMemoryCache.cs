using Cache.Abstraction;
using Cache.Models;
using System.Collections.Concurrent;

namespace Cache.Memory;

public sealed class LruMemoryCache<TValue> : IMemoryCache<TValue>
{
    private static LruMemoryCache<TValue>? _lruMemoryCache = null;
    private readonly ConcurrentDictionary<string, LinkedListNode<(string Key, CacheItem<TValue> Item)>> _cacheStore;
    private readonly LinkedList<(string Key, CacheItem<TValue> Item)> _lruList;
    private readonly int _capacity;

    private LruMemoryCache(int capacity)
    {
        _cacheStore = new();
        _lruList = new();
        _capacity = capacity;
    }

    public static LruMemoryCache<TValue> Instance(int capacity)
    {
        if (_lruMemoryCache == null)
        {
            _lruMemoryCache = new LruMemoryCache<TValue>(capacity);
        }
        return _lruMemoryCache;
    }

    public void Set(string key, TValue value, TimeSpan duration)
    {
        if (_cacheStore.TryGetValue(key, out var existingNode))
        {
            _lruList.Remove(existingNode);
        }
        else
        {
            if (_cacheStore.Count > _capacity)
            {
                // Evict LRU Cache
                var lastNode = _lruList.Last;
                if (lastNode != null)
                {
                    _cacheStore.Remove(lastNode.Value.Key, out _);
                    _lruList.Remove(lastNode);
                }
            }
        }

        var newItem = new CacheItem<TValue>(value, duration);
        var newNode = new LinkedListNode<(string Key, CacheItem<TValue> Item)>((key, newItem));
        _lruList.AddFirst(newNode);
        _cacheStore[key] = newNode;
    }

    public TValue Get(string key)
    {
        if (_cacheStore.TryGetValue(key, out var cacheNode))
        {
            if (!cacheNode.Value.Item.IsExpired)
            {
                var cacheItem = cacheNode.Value.Item;
                cacheItem.RefreshExpiration();

                _lruList.Remove(cacheNode);
                var newNode = new LinkedListNode<(string Key, CacheItem<TValue> Item)>((key, cacheItem));
                _lruList.AddFirst(newNode);
                _cacheStore[key] = newNode;

                return cacheItem.Value;
            }
            else
            {
                _cacheStore.TryRemove(key, out _);
                _lruList.Remove(cacheNode);
            }
        }

        throw new KeyNotFoundException("Item not found or expired.");
    }

    public bool TryGetValue(string key, out TValue? value)
    {
        value = default;

        if (_cacheStore.TryGetValue(key, out var cacheNode))
        {
            if (!cacheNode.Value.Item.IsExpired)
            {
                // Refresh expiration and move node to head (mark as recently used)
                var cacheItem = cacheNode.Value.Item;
                cacheItem.RefreshExpiration();

                _lruList.Remove(cacheNode);
                var newNode = new LinkedListNode<(string Key, CacheItem<TValue> Item)>((key, cacheItem));
                _lruList.AddFirst(newNode);
                _cacheStore[key] = newNode;

                value = cacheItem.Value;
                return true;
            }
            else
            {
                // Remove expired item
                _cacheStore.TryRemove(key, out _);
                _lruList.Remove(cacheNode);
            }
        }

        return false;
    }

    public bool Remove(string key)
    {
        if (_cacheStore.TryRemove(key, out var cacheNode))
        {
            _lruList.Remove(cacheNode);
            return true;
        }

        return false;
    }

    public void RemoveAll()
    {
        _cacheStore.Clear();
        _lruList.Clear();
    }
}
