using Microsoft.Extensions.Options;
using Server.Application.Storage;

namespace Server.Data.Storage;

//Luka Kanjir
public sealed class LocalFileStorage : IFileStorage
{
    private readonly StorageOptions options;
    private readonly HashSet<string> allowedTypes;
    private readonly string rootPath;

    public LocalFileStorage(IOptions<StorageOptions> options)
    {
        this.options = options.Value;
        rootPath = Path.GetFullPath(this.options.RootPath);
        Directory.CreateDirectory(rootPath);
        allowedTypes = new HashSet<string>(this.options.AllowedTypes, StringComparer.OrdinalIgnoreCase);
    }

    public async Task<string> SaveAsync(ImageType type, Stream content, string contentType,
        CancellationToken ct = default)
    {
        if (!allowedTypes.Contains(contentType)) throw new InvalidOperationException("Tip podatka nije podržan");

        var ext = ImageTools.GetExtensionForContentType(contentType);
        var fileName = $"{Guid.NewGuid():N}{ext}";
        var subfolder = TypeToFolder(type);
        var relativePath = Path.Combine(subfolder, fileName).Replace('\\', '/');
        var fullPath = GetSafePath(relativePath);

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await using var writeStream = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None,
            81920,
            FileOptions.Asynchronous);
        await content.CopyToAsync(writeStream, ct);

        return relativePath;
    }

    private string GetSafePath(string relativePath)
    {
        var safeRelative = relativePath.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var combined = Path.GetFullPath(Path.Combine(rootPath, safeRelative));
        if (!combined.StartsWith(rootPath, StringComparison.Ordinal))
            throw new InvalidOperationException("Greška u sastavljanju putanje");

        return combined;
    }

    private static string TypeToFolder(ImageType type) => type switch
    {
        ImageType.Events => "events",
        ImageType.Faults => "faults",
        ImageType.Profiles => "profiles",
        _ => "other"
    };

    public Task<(Stream Content, string ContentType)?> OpenReadAsync(string relativePath,
        CancellationToken ct = default)
    {
        var fullPath = GetSafePath(relativePath);
        if (!File.Exists(fullPath))
            return Task.FromResult<(Stream Content, string ContentType)?>(null);

        var readStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 81920,
            FileOptions.Asynchronous);
        var contentType = ImageTools.GetContentTypeFromExtension(Path.GetExtension(fullPath)) ??
                          "application/octet-stream";
        return Task.FromResult<(Stream Content, string ContentType)?>((readStream, contentType));
    }
}