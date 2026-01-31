namespace Client.Application.Images;



//Luka Kanjir
public interface IImageService
{
    Task<ImagePayload?> GetProfileImageAsync(int userId, CancellationToken ct = default);
    void InvalidateProfile(int userId);
}