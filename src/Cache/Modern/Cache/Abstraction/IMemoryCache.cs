namespace Cache.Abstraction;

public interface IMemoryCache<TValue>
{
    void Set(string key, TValue value, TimeSpan duration);
    TValue Get(string key);
    bool TryGetValue(string key, out TValue? value);
    bool Remove(string key);
    void RemoveAll();
}
