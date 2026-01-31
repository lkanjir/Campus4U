namespace Server.Application.Storage;

public sealed record ImageUpload(Stream Content, string ContentType, long Length);
public sealed record ImageContent(Stream Content, string ContentType);

public interface IImageService
{
    Task<string> UploadEventAsync(int eventId, ImageUpload upload, CancellationToken ct = default);
    Task<ImageContent> GetEventImageAsync(int eventId, CancellationToken ct = default);
}