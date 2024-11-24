namespace LeastFrequentlyUsed;

public class CacheNode
{
    public CacheNode(int key, int value)
    {
        Key = key;
        Value = value;
        Frequency = 1;
    }

    public int Key { get; set; }
    public int Value { get; set; }
    public int Frequency { get; set; }
    public CacheNode Next { get; set; }
    public CacheNode Previous { get; set; }
}

