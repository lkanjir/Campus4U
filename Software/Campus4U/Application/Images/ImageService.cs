namespace Client.Application.Images;

public sealed class ImageService(IImageSource source, ImageCache cache, TimeSpan ttl) : IImageService
{
    public async Task<ImagePayload?> GetProfileImageAsync(int userId, CancellationToken ct = default)
    {
        if (userId <= 0) return null;

        var key = new ImageKey(ImageType.Profile, userId);
        if (cache.TryGet(key, out var cached)) return cached;

        var payload = await source.GetProfileImageAsync(userId, ct);
        if (payload is not null) cache.Set(key, payload, ttl);

        return payload;
    }

    public void InvalidateProfile(int userId) => cache.Invalidate(new ImageKey(ImageType.Profile, userId));
}