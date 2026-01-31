namespace Server.Application.Storage;

//Luka Kanjir
public static class ImageTools
{
    public static string GetExtensionForContentType(string contentType) => contentType switch
    {
        "image/jpeg" => ".jpg",
        "image/png" => ".png",
        "image/webp" => ".webp",
        _ => throw new InvalidOperationException("Tip podatka nije podržan")
    };

    public static string? GetContentTypeFromExtension(string extension) => extension.ToLowerInvariant() switch
    {
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".webp" => "image/webp",
        _ => null
    };

    public static bool IsAllowedContentType(string? contentType, IReadOnlyCollection<string> allowedTypes) =>
        !string.IsNullOrWhiteSpace(contentType) && allowedTypes.Contains(contentType);

    public static string? DetectContentType(ReadOnlySpan<byte> header)
    {
        return header.Length switch
        {
            >= 3 when (header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF) => "image/jpeg",
            >= 8 when (header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47 &&
                       header[4] == 0x0D && header[5] == 0x0A && header[6] == 0x1A && header[7] == 0x0A) => "image/png",
            >= 12 when (header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46 &&
                        header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 &&
                        header[11] == 0x50) => "image/webp",
            _ => null
        };
    }
}