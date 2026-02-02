namespace Client.Application.Images;

//Luka Kanjir
public sealed class ImageService(IImageSource source, ImageCache cache, TimeSpan ttl) : IImageService
{
    public async Task<ImagePayload?> GetProfileImageAsync(int userId, CancellationToken ct = default)
    {
        var key = new ImageKey(ImageType.Profile, userId);
        if (cache.TryGet(key, out var cached)) return cached;

        var payload = await source.GetProfileImageAsync(userId, ct);
        if (payload is not null) cache.Set(key, payload, ttl);

        return payload;
    }

    public async Task UploadProfileImageAsync(int userId, ImageUpload upload, CancellationToken ct = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);

        await source.UploadProfileImageAsync(upload, ct);
        cache.Invalidate(new ImageKey(ImageType.Profile, userId));
    }

    public void InvalidateProfile(int userId) => cache.Invalidate(new ImageKey(ImageType.Profile, userId));

    public async Task<ImagePayload?> GetEventImageAsync(int eventId, CancellationToken ct = default)
    {
        var key = new ImageKey(ImageType.Event, eventId);
        if (cache.TryGet(key, out var cached)) return cached;

        var payload = await source.GetEventImageAsync(eventId, ct);
        if (payload is not null) cache.Set(key, payload, ttl);

        return payload;
    }

    public async Task UploadEventImageAsync(int eventId, ImageUpload upload, CancellationToken ct = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(eventId);
        await source.UploadEventImageAsync(eventId, upload, ct);
        cache.Invalidate(new ImageKey(ImageType.Event, eventId));
    }

    public void InvalidateEvent(int eventId) => cache.Invalidate(new ImageKey(ImageType.Event, eventId));

    public async Task<ImagePayload?> GetFaultImageAsync(int faultId, CancellationToken ct = default)
    {
        var key = new ImageKey(ImageType.Fault, faultId);
        if (cache.TryGet(key, out var cached)) return cached;

        var payload = await source.GetFaultImageAsync(faultId, ct);
        if (payload is not null) cache.Set(key, payload, ttl);

        return payload;
    }

    public async Task UploadFaultImageAsync(int faultId, ImageUpload upload, CancellationToken ct = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(faultId);
        await source.UploadFaultImageAsync(faultId, upload, ct);
        cache.Invalidate(new ImageKey(ImageType.Fault, faultId));
    }

    public void InvalidateFault(int faultId) => cache.Invalidate(new ImageKey(ImageType.Fault, faultId));
}