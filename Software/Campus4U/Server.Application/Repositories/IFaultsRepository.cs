namespace Server.Application.Repositories;

//Luka Kanjir
public sealed record FaultImageInfo(int FaultId, int OwnerUserId, string? ImagePath);

//Luka Kanjir
public interface IFaultsRepository
{
    Task<FaultImageInfo?> GetImageInfoAsync(int faultId, CancellationToken ct = default);
    Task<bool> SetImagePathAsync(int faultId, string imagePath, CancellationToken ct = default);
}