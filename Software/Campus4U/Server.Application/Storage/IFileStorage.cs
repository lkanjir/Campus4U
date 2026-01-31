namespace Server.Application.Storage;

//Luka Kanjir
public interface IFileStorage
{
    Task<string> SaveAsync(ImageType type, Stream content, string contentType, CancellationToken ct = default);
    Task<(Stream Content, string ContentType)?> OpenReadAsync(string relativePath, CancellationToken ct = default);
    Task DeleteAsync(string relativePath, CancellationToken ct = default);
}