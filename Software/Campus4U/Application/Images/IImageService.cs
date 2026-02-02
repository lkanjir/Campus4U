namespace Client.Application.Images;

//Luka Kanjir
public interface IImageService
{
    Task<ImagePayload?> GetProfileImageAsync(int userId, CancellationToken ct = default);
    Task UploadProfileImageAsync(int userId, ImageUpload upload, CancellationToken ct = default);
    void InvalidateProfile(int userId);
    Task<ImagePayload?> GetEventImageAsync(int eventId, CancellationToken ct = default);
    Task UploadEventImageAsync(int eventId, ImageUpload upload, CancellationToken ct = default);
    void InvalidateEvent(int eventId);
    Task<ImagePayload?> GetFaultImageAsync(int faultId, CancellationToken ct = default);
    Task UploadFaultImageAsync(int faultId, ImageUpload upload, CancellationToken ct = default);
    void InvalidateFault(int faultId);
}