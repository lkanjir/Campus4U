namespace Server.Application.Repositories;

//Luka Kanjir
public sealed record UserAuthInfo(int UserId, bool IsStaff, string? ProfileImagePath);

public interface IUsersRepository
{
    Task<UserAuthInfo?> GetBySubAsync(string sub, CancellationToken ct = default);
    Task<bool> SetProfileImagePathAsync(int userId, string imagePath, CancellationToken ct = default);
    Task<string?> GetProfileImagePathAsync(int userId, CancellationToken ct = default);
}