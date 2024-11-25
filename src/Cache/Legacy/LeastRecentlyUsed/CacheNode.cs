namespace LeastRecentlyUsed;

internal sealed class CacheNode
{
    public CacheNode(int key, int value)
    {
        Key = key;
        Value = value;
    }

    public int Key { get; set; }
    public int Value { get; set; }
}

