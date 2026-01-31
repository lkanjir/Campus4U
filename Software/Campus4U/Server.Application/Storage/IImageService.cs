namespace Server.Application.Storage;

//Luka Kanjir
public sealed record ImageUpload(Stream Content, string ContentType, long Length);

//Luka Kanjir
public sealed record ImageContent(Stream Content, string ContentType);

//Luka Kanjir
public interface IImageService
{
    Task<string> UploadEventAsync(int eventId, ImageUpload upload, CancellationToken ct = default);
    Task<ImageContent> GetEventImageAsync(int eventId, CancellationToken ct = default);
    Task<string> UploadFaultAsync(int faultId, ImageUpload upload, string userSub, CancellationToken ct = default);
    Task<ImageContent> GetFaultImageAsync(int faultId, string userSub, CancellationToken ct = default);
    Task<string> UploadProfileAsync(ImageUpload upload, string userSub, CancellationToken ct = default);
    Task<ImageContent> GetProfileImageAsync(int userId, CancellationToken ct = default);
}