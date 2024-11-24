using LeastFrequentlyUsed;

LeastFrequentlyUsedCache cache = new(2);
cache.Put(3, 1);
cache.Put(2, 1);
cache.Put(2, 2);
cache.Put(4, 4);
cache.Get(2);
Console.WriteLine("Operations are done.");
