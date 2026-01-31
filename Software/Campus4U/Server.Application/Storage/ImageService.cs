using Microsoft.Extensions.Logging;
using Server.Application.Repositories;

namespace Server.Application.Storage;

public sealed class ImageService(
    IEventsRepository eventsRepository,
    IFaultsRepository faultsRepository,
    IUsersRepository usersRepository,
    IFileStorage storage,
    StorageOptions options,
    ILogger<ImageService> logger)
    : IImageService
{
    public async Task<string> UploadEventAsync(int eventId, ImageUpload upload, CancellationToken ct = default)
    {
        var info = await eventsRepository.GetImageInfoAsync(eventId, ct);
        if (info == null) throw new ImageException(ImageErrorCode.NotFound, "Event ne postoji");

        var oldPath = info.ImagePath;
        var path = await SaveValidatedAsync(ImageType.Events, upload, ct);

        var saved = await eventsRepository.SetImagePathAsync(eventId, path, ct);
        if (!saved)
        {
            await TryDeleteAsync(path, ct);
            throw new ImageException(ImageErrorCode.NotFound, "Event ne postoji");
        }

        if (!string.IsNullOrWhiteSpace(oldPath) && !string.Equals(oldPath, path, StringComparison.Ordinal))
            await TryDeleteAsync(oldPath, ct);

        return path;
    }

    private async Task TryDeleteAsync(string relativePath, CancellationToken ct)
    {
        try
        {
            await storage.DeleteAsync(relativePath, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Brisanje slike nije uspjelo: {relativePath}", relativePath);
        }
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

    public async Task<string> UploadFaultAsync(int faultId, ImageUpload upload, string userSub,
        CancellationToken ct = default)
    {
        var user = await usersRepository.GetBySubAsync(userSub, ct);
        if (user is null)
            throw new ImageException(ImageErrorCode.Unauthorized, "Nemate pravo pristupa slici, prijavite se");

        var fault = await faultsRepository.GetImageInfoAsync(faultId, ct);
        if (fault is null) throw new ImageException(ImageErrorCode.NotFound, "Slika ne postoji");
        if (!CanAccessFault(user, fault)) throw new ImageException(ImageErrorCode.Forbidden, "Zabranjen pristup");

        var oldPath = fault.ImagePath;
        var path = await SaveValidatedAsync(ImageType.Faults, upload, ct);
        var saved = await faultsRepository.SetImagePathAsync(faultId, path, ct);
        if (!saved)
        {
            await TryDeleteAsync(path, ct);
            throw new ImageException(ImageErrorCode.NotFound, "Kvar ne postoji");
        }

        if (!string.IsNullOrWhiteSpace(oldPath) && !string.Equals(oldPath, path, StringComparison.Ordinal))
            await TryDeleteAsync(oldPath, ct);

        return path;
    }

    private static bool CanAccessFault(UserAuthInfo user, FaultImageInfo fault) =>
        user.UserId == fault.OwnerUserId || user.IsStaff;

    public async Task<ImageContent> GetFaultImageAsync(int faultId, string userSub, CancellationToken ct = default)
    {
        var user = await usersRepository.GetBySubAsync(userSub, ct);
        if (user is null)
            throw new ImageException(ImageErrorCode.Unauthorized, "Nemate pravo pristupa slici, prijavite se");

        var fault = await faultsRepository.GetImageInfoAsync(faultId, ct);
        if (fault is null || string.IsNullOrWhiteSpace(fault.ImagePath))
            throw new ImageException(ImageErrorCode.NotFound, "Slika ne postoji");
        if (!CanAccessFault(user, fault)) throw new ImageException(ImageErrorCode.Forbidden, "Zabranjen pristup");

        var result = await storage.OpenReadAsync(fault.ImagePath, ct);
        if (result is null) throw new ImageException(ImageErrorCode.NotFound, "Slika ne postoji");

        return new ImageContent(result.Value.Content, result.Value.ContentType);
    }

    public async Task<string> UploadProfileAsync(ImageUpload upload, string userSub, CancellationToken ct = default)
    {
        var user = await usersRepository.GetBySubAsync(userSub, ct);
        if (user is null)
            throw new ImageException(ImageErrorCode.Unauthorized, "Nemate pravo pristupa slici, prijavite se");

        var oldPath = user.ProfileImagePath;
        var path = await SaveValidatedAsync(ImageType.Profiles, upload, ct);
        var saved = await usersRepository.SetProfileImagePathAsync(user.UserId, path, ct);
        if (!saved)
        {
            await TryDeleteAsync(path, ct);
            throw new ImageException(ImageErrorCode.NotFound, "Korisnik ne postoji");
        }

        if (!string.IsNullOrWhiteSpace(oldPath) && !string.Equals(oldPath, path, StringComparison.Ordinal))
            await TryDeleteAsync(oldPath, ct);

        return path;
    }

    public async Task<ImageContent> GetProfileImageAsync(int userId, CancellationToken ct = default)
    {
        var path = await usersRepository.GetProfileImagePathAsync(userId, ct);
        if (string.IsNullOrWhiteSpace(path))
            throw new ImageException(ImageErrorCode.NotFound, "Slika ne postoji");

        var result = await storage.OpenReadAsync(path, ct);
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