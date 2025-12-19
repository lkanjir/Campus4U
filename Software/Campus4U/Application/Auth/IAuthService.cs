using Client.Domain.Auth;

namespace Client.Application.Auth;

//Luka Kanjir
public sealed record TokenGrantResult(
    bool IsSuccess,
    string? AccessToken,
    string? RefreshToken,
    DateTimeOffset? ExpiresAt,
    string? Error
);

//Luka Kanjir
public interface IAuthService
{
    Task<AuthSessionRestoreResult> RestoreSessionAsync(CancellationToken ct = default);
    Task<TokenGrantResult> LoginAsync(CancellationToken cancellationToken = default);
    Task LogoutAsync(CancellationToken ct = default);
}