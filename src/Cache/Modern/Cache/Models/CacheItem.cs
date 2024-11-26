namespace Cache.Models;

public class CacheItem<T>(T value, TimeSpan duration)
{
    public T Value { get; set; } = value;
    public DateTime Expiration { get; set; } = DateTime.UtcNow.Add(duration);
    public bool IsExpired => DateTime.UtcNow > Expiration;
    private TimeSpan Duration => duration;
    public void RefreshExpiration() => Expiration = DateTime.UtcNow.Add(Duration);
}
