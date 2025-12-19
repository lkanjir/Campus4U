using Client.Domain.Auth;

namespace Client.Application.Auth;

public sealed record TokenGrantResult(
    bool IsSuccess,
    string? AccessToken,
    string? RefreshToken,
    DateTimeOffset? ExpiresAt,
    string? Error
);

public interface IAuthService
{
    Task<AuthSessionRestoreResult> RestoreSessionAsync(CancellationToken ct = default);
    Task<TokenGrantResult> LoginAsync(CancellationToken cancellationToken = default);
    Task LogoutAsync(CancellationToken ct = default);
}