namespace Server.Application.Events;

public sealed record EventImageInfo(int EventId, string? ImagePath);

public interface IEventsRepository
{
    Task<EventImageInfo?> GetImageInfoAsync(int eventId, CancellationToken ct = default);
    Task<bool> SetImagePathAsync(int eventId, string imagePath, CancellationToken ct = default);
}