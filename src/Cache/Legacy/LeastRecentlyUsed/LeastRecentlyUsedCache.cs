namespace LeastRecentlyUsed;

public sealed class LeastRecentlyUsedCache
{
    private static LeastRecentlyUsedCache? _cacheInstance = null;

    private readonly int _capacity;
    private readonly Dictionary<int, LinkedListNode<CacheNode>> _cacheStore;
    private readonly LinkedList<CacheNode> _nodeList;
    private readonly LinkedListNode<CacheNode> _header;
    private readonly LinkedListNode<CacheNode> _tail;

    private LeastRecentlyUsedCache(int capacity)
    {
        _capacity = capacity;
        _cacheStore = [];
        _nodeList = new();
        _header = new(new(-1, -1));
        _tail = new(new(-1, -1));
        _nodeList.AddFirst(_header);
        _nodeList.AddLast(_tail);
    }

    public static LeastRecentlyUsedCache Instance(int capacity)
    {
        if (_cacheInstance == null)
        {
            _cacheInstance = new(capacity);
        }

        return _cacheInstance;
    }

    public int Get(int key)
    {
        if (!_cacheStore.TryGetValue(key, out LinkedListNode<CacheNode>? node))
            return -1;

        _cacheStore.Remove(key);
        _nodeList.Remove(node);
        _nodeList.AddAfter(_header, node);
        _cacheStore.Add(key, _header.Next!);
        return node.Value.Value;
    }

    public void Set(int key, int value)
    {
        if (_cacheStore.TryGetValue(key, out LinkedListNode<CacheNode>? node))
        {
            _cacheStore.Remove(key);
            _nodeList.Remove(node);
        }

        if (_cacheStore.Count == _capacity)
        {
            _cacheStore.Remove(_tail.Previous!.Value.Key);
            _nodeList.Remove(_tail.Previous);
        }

        LinkedListNode<CacheNode> newNode = new(new(key, value));
        _nodeList.AddAfter(_header, newNode);
        _cacheStore.Add(key, _header.Next!);
    }

    public bool Remove(int key)
    {
        if (_cacheStore.TryGetValue(key, out LinkedListNode<CacheNode>? node))
        {
            _cacheStore.Remove(key);
            _nodeList.Remove(node);

            return true;
        }

        return false;
    }

    public void RemoveAll()
    {
        _cacheStore.Clear();
        _nodeList.Clear();
    }
}

