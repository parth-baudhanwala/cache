using System.Linq;

namespace LeastFrequentlyUsed;

public class LeastFrequentlyUsedCache
{
    private readonly Dictionary<int, CacheNode> _cache;
    private readonly Dictionary<int, LinkedList<CacheNode>> _frequencyMap;
    private readonly int _capacity;
    private int _minFrequency;

    public LeastFrequentlyUsedCache(int capacity)
    {
        _capacity = capacity;
        _cache = new();
        _frequencyMap = new();
    }

    public int Get(int key)
    {
        if (!_cache.TryGetValue(key, out CacheNode node))
        {
            return -1;
        }

        UpdateFrequency(node);
        return node.Value;
    }

    public void Put(int key, int value)
    {
        if (_cache.TryGetValue(key, out CacheNode node))
        {
            node.Value = value;
            UpdateFrequency(node);
            return;
        }

        if (_cache.Count == _capacity)
        {
            var linkedList = _frequencyMap[_minFrequency];
            _cache.Remove(linkedList.Last().Key);
            linkedList.Remove(linkedList.Last());
        }

        _minFrequency = 1;

        if (!_frequencyMap.TryGetValue(_minFrequency, out var firstLinkedList))
        {
            firstLinkedList = new();
        }

        node = new(key, value);
        firstLinkedList.AddFirst(node);

        if(_cache.ContainsKey(key))
        {
            _cache[key] = node;
        }
        else
        {
            _cache.Add(key, node);
        }

        if (_frequencyMap.ContainsKey(_minFrequency))
        {
            _frequencyMap[_minFrequency] = firstLinkedList;
        }
        else
        {
            _frequencyMap.Add(_minFrequency, firstLinkedList);
        }
    }

    private void UpdateFrequency(CacheNode node)
    {
        _cache.Remove(node.Key);

        if(_frequencyMap.TryGetValue(node.Frequency, out var currentList))
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

        if (_cache.ContainsKey(node.Key))
        {
            _cache[node.Key] = node;
        }
        else
        {
            _cache.Add(node.Key, node);
        }

        if (_frequencyMap.ContainsKey(node.Frequency))
        {
            _frequencyMap[node.Frequency] = nextList;
        }
        else
        {
            _frequencyMap.Add(node.Frequency, nextList);
        }
    }
}

