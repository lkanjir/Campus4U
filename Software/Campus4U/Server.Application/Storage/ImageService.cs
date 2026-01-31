using Microsoft.VisualBasic;
using Server.Application.Events;

namespace Server.Application.Storage;

public sealed class ImageService(IEventsRepository eventsRepository, IFileStorage storage, StorageOptions options)
    : IImageService
{
    public async Task<string> UploadEventAsync(int eventId, ImageUpload upload, CancellationToken ct = default)
    {
        var info = await eventsRepository.GetImageInfoAsync(eventId, ct);
        if (info == null) throw new ImageException(ImageErrorCode.NotFound, "Event ne postoji");

        var path = await SaveValidatedAsync(ImageType.Events, upload, ct);

        var saved = await eventsRepository.SetImagePathAsync(eventId, path, ct);
        if (!saved) throw new ImageException(ImageErrorCode.NotFound, "Event ne postoji");

        return path;
    }

    private async Task<string> SaveValidatedAsync(ImageType type, ImageUpload upload, CancellationToken ct)
    {
        if (upload.Length <= 0) throw new ImageException(ImageErrorCode.Invalid, "Slika je obavezna");
        if (upload.Length > options.MaxBytes)
            throw new ImageException(ImageErrorCode.TooLarge, "Slika je prevelike veličine");
        if (!ImageTools.IsAllowedContentType(upload.ContentType, options.AllowedTypes))
            throw new ImageException(ImageErrorCode.UnsupportedType, "Tip slike nije podržan");

        var prepared = await PrepareStreamAsync(upload, ct);
        try
        {
            return await storage.SaveAsync(type, prepared, upload.ContentType, ct);
        }
        finally
        {
            if (!ReferenceEquals(prepared, upload.Content))
                await prepared.DisposeAsync();
        }
    }

    public async Task<ImageContent> GetEventImageAsync(int eventId, CancellationToken ct = default)
    {
        var info = await eventsRepository.GetImageInfoAsync(eventId, ct);
        if (info is null || string.IsNullOrWhiteSpace(info.ImagePath))
            throw new ImageException(ImageErrorCode.NotFound, "Slika ne postoji");

        var result = await storage.OpenReadAsync(info.ImagePath, ct);
        if (result is null) throw new ImageException(ImageErrorCode.NotFound, "Slika ne postoji");

        return new ImageContent(result.Value.Content, result.Value.ContentType);
    }

    private async Task<Stream> PrepareStreamAsync(ImageUpload upload, CancellationToken ct = default)
    {
        var stream = upload.Content;
        var header = new byte[12];
        var read = await stream.ReadAsync(header.AsMemory(0, header.Length), ct);
        var detected = ImageTools.DetectContentType(header.AsSpan(0, read));

        if (detected is null || !string.Equals(detected, upload.ContentType, StringComparison.OrdinalIgnoreCase))
            throw new ImageException(ImageErrorCode.Invalid, "Sadržaj slike nije ispravan");

        if (stream.CanSeek)
        {
            stream.Position = 0;
            return stream;
        }

        var buffered = new MemoryStream();
        await buffered.WriteAsync(header.AsMemory(0, read), ct);
        await stream.CopyToAsync(buffered, ct);
        buffered.Position = 0;
        return buffered;
    }
}