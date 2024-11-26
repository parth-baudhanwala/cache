using Cache.Abstraction;
using Cache.Models;
using System.Collections.Concurrent;

namespace Cache.Memory;

public sealed class MemoryCache<TValue> : IMemoryCache<TValue>
{
    private static MemoryCache<TValue>? _memoryCache = null;
    private readonly ConcurrentDictionary<string, CacheItem<TValue>> _cacheStore;

    private MemoryCache()
    {
        _cacheStore = new();
    }

    public static MemoryCache<TValue> Instance
    {
        get
        {
            if (_memoryCache == null)
            {
                _memoryCache = new MemoryCache<TValue>();
            }
            return _memoryCache;
        }
    }

    public void Set(string key, TValue value, TimeSpan duration)
    {
        _cacheStore[key] = new CacheItem<TValue>(value, duration);
    }

    public TValue Get(string key)
    {
        if (_cacheStore.TryGetValue(key, out var cacheItem))
        {
            if (!cacheItem.IsExpired)
            {
                return cacheItem.Value;
            }

            _cacheStore.TryRemove(key, out _);
        }

        throw new KeyNotFoundException("Item not found or expired.");
    }

    public bool TryGetValue(string key, out TValue? value)
    {
        value = default;

        if (_cacheStore.TryGetValue(key, out var cacheItem))
        {
            if (!cacheItem.IsExpired)
            {
                value = cacheItem.Value;
                return true;
            }

            _cacheStore.TryRemove(key, out _);
        }

        return false;
    }

    public bool Remove(string key)
    {
        return _cacheStore.TryRemove(key, out _);
    }

    public void RemoveAll()
    {
        _cacheStore.Clear();
    }
}
