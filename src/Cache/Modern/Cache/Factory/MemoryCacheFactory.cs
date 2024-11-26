using Cache.Abstraction;
using Cache.Enums;
using Cache.Memory;

namespace Cache.Factory;

public static class MemoryCacheFactory<TValue>
{
    public static IMemoryCache<TValue> Create()
    {
        return MemoryCache<TValue>.Instance;
    }

    public static IMemoryCache<TValue> Create(CachePolicy policy, int capacity)
    {
        if (policy == CachePolicy.LRU)
        {
            return LruMemoryCache<TValue>.Instance(capacity);
        }

        return MemoryCache<TValue>.Instance;
    }
}
