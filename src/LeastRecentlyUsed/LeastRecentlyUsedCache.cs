namespace LeastRecentlyUsed;

public class LeastRecentlyUsedCache
{
    private readonly int _capacity;
    private readonly Dictionary<int, LinkedListNode<CacheNode>> _keyValuePairs;
    private readonly LinkedList<CacheNode> _nodeList;
    private readonly LinkedListNode<CacheNode> _header;
    private readonly LinkedListNode<CacheNode> _tail;

    public LeastRecentlyUsedCache(int capacity)
    {
        _capacity = capacity;
        _keyValuePairs = new();
        _nodeList = new();
        _header = new(new(-1, -1));
        _tail = new(new(-1, -1));
        _nodeList.AddFirst(_header);
        _nodeList.AddLast(_tail);
    }

    public int Get(int key)
    {
        if (!_keyValuePairs.TryGetValue(key, out LinkedListNode<CacheNode> node))
            return -1;

        _keyValuePairs.Remove(key);
        _nodeList.Remove(node);
        _nodeList.AddAfter(_header, node);
        _keyValuePairs.Add(key, _header.Next);
        return node.Value.Value;
    }

    public void Put(int key, int value)
    {
        if (_keyValuePairs.TryGetValue(key, out LinkedListNode<CacheNode> node))
        {
            _keyValuePairs.Remove(key);
            _nodeList.Remove(node);
        }

        if (_keyValuePairs.Count == _capacity)
        {
            _keyValuePairs.Remove(_tail.Previous.Value.Key);
            _nodeList.Remove(_tail.Previous);
        }

        LinkedListNode<CacheNode> newNode = new(new(key, value));
        _nodeList.AddAfter(_header, newNode);
        _keyValuePairs.Add(key, _header.Next);
    }
}

