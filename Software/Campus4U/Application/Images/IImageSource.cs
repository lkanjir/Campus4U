namespace Client.Application.Images;

//Luka Kanjir
public interface IImageSource
{
    Task<ImagePayload?> GetProfileImageAsync(int userId, CancellationToken ct = default);
    Task UploadProfileImageAsync(ImageUpload upload, CancellationToken ct = default);
    Task<ImagePayload?> GetEventImageAsync(int eventId, CancellationToken ct = default);
    Task UploadEventImageAsync(int eventId, ImageUpload upload, CancellationToken ct = default);
}