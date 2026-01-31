namespace Client.Application.Images;

//Luka Kanjir
public interface IImageSource
{
    Task<ImagePayload?> GetProfileImageAsync(int userId, CancellationToken ct = default);
}