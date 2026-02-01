using System.Collections.Concurrent;

namespace Client.Application.Images;

//Luka Kanjir
public sealed class ImageCache
{
    private sealed record CacheEntry(ImagePayload Payload, DateTimeOffset ExpiresAt);
    private readonly ConcurrentDictionary<ImageKey, CacheEntry> cache = new();

    public bool TryGet(ImageKey key, out ImagePayload? payload)
    {
        payload = null;
        if (!cache.TryGetValue(key, out var entry)) return false;
        if (entry.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            cache.TryRemove(key, out _);
            return false;
        }

        payload = entry.Payload;
        return true;
    }

    public void Set(ImageKey key, ImagePayload payload, TimeSpan ttl)
    {
        var entry = new CacheEntry(payload, DateTimeOffset.UtcNow.Add(ttl));
        cache[key] = entry;
    }

    public void Invalidate(ImageKey key) => cache.TryRemove(key, out _);
}