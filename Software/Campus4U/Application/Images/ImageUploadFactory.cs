namespace Client.Application.Images;

public static class ImageUploadFactory
{
    public static async Task<(ImageUploadResult? Result, string? Error)> TryCreateAsync(
        string filePath, long maxBytes, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return (null, "Neispravna putanja do datoteke.");

        var fileInfo = new FileInfo(filePath);
        if (!fileInfo.Exists)
            return (null, "Datoteka ne postoji.");

        if (fileInfo.Length is <= 0 || fileInfo.Length > maxBytes)
        {
            var maxMb = Math.Max(1, maxBytes / (1024 * 1024));
            return (null, $"Slika je prevelika ili nema slike (max {maxMb}MB).");
        }

        var contentType = GetContentType(fileInfo.Extension);
        if (contentType is null)
            return (null, "Nepodržan format slike.");

        var bytes = await File.ReadAllBytesAsync(filePath, ct);
        var stream = new MemoryStream(bytes, writable: false);
        var upload = new ImageUpload(stream, contentType, bytes.LongLength, fileInfo.Name);
        return (new ImageUploadResult(upload, bytes), null);
    }

    private static string? GetContentType(string extension) =>
        extension.Trim().ToLowerInvariant() switch
        {
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => null
        };
}