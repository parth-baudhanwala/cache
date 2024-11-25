namespace LeastFrequentlyUsed;

public sealed class LeastFrequentlyUsedCache
{
    private static LeastFrequentlyUsedCache? _cacheInstance = null;

    private readonly Dictionary<int, CacheNode> _cacheStore;
    private readonly Dictionary<int, LinkedList<CacheNode>> _frequencyMap;
    private readonly int _capacity;
    private int _minFrequency;

    private LeastFrequentlyUsedCache(int capacity)
    {
        _capacity = capacity;
        _cacheStore = [];
        _frequencyMap = [];
    }

    public static LeastFrequentlyUsedCache Instance(int capacity)
    {
        if (_cacheInstance == null)
        {
            _cacheInstance = new(capacity);
        }

        return _cacheInstance;
    }

    public int Get(int key)
    {
        if (!_cacheStore.TryGetValue(key, out CacheNode? node))
        {
            return -1;
        }

        UpdateFrequency(node);
        return node.Value;
    }

    public void Set(int key, int value)
    {
        if (_cacheStore.TryGetValue(key, out CacheNode? node))
        {
            node.Value = value;
            UpdateFrequency(node);
            return;
        }

        if (_cacheStore.Count == _capacity)
        {
            var linkedList = _frequencyMap[_minFrequency];

            if (linkedList.Last != null)
            {
                _cacheStore.Remove(linkedList.Last.Value.Key);
                linkedList.Remove(linkedList.Last.Value);
            }
        }

        _minFrequency = 1;

        if (!_frequencyMap.TryGetValue(_minFrequency, out var firstLinkedList))
        {
            firstLinkedList = new();
        }

        node = new(key, value);
        firstLinkedList.AddFirst(node);

        if (!_cacheStore.TryAdd(key, node))
        {
            _cacheStore[key] = node;
        }

        if (!_frequencyMap.TryAdd(_minFrequency, firstLinkedList))
        {
            _frequencyMap[_minFrequency] = firstLinkedList;
        }
    }

    public bool Remove(int key)
    {
        if (_cacheStore.TryGetValue(key, out CacheNode? node))
        {
            _cacheStore.Remove(key);
            _frequencyMap.Remove(node.Frequency);

            return true;
        }

        return false;
    }

    public void RemoveAll()
    {
        _cacheStore.Clear();
        _frequencyMap.Clear();
    }

    private void UpdateFrequency(CacheNode node)
    {
        _cacheStore.Remove(node.Key);

        if (_frequencyMap.TryGetValue(node.Frequency, out var currentList))
            currentList.Remove(node);

        if (node.Frequency == _minFrequency && (currentList == null || currentList.Count == 0))
        {
            _minFrequency++;
        }

        if (!_frequencyMap.TryGetValue(node.Frequency + 1, out var nextList))
        {
            nextList = new();
        }

        node.Frequency++;
        nextList.AddFirst(node);

        if (!_cacheStore.TryAdd(node.Key, node))
        {
            _cacheStore[node.Key] = node;
        }

        if (!_frequencyMap.TryAdd(node.Frequency, nextList))
        {
            _frequencyMap[node.Frequency] = nextList;
        }
    }
}

