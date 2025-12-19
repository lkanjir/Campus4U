using Client.Domain.Auth;

namespace Client.Application.Auth;

//Luka Kanjir
public sealed class AuthService(IAuthProvider authProvider, ITokenStore store) : IAuthService
{
    private readonly IAuthProvider authProvider = authProvider;
    private readonly TimeSpan expiredGracePeriod = TimeSpan.FromMinutes(1);

    public async Task<AuthSessionRestoreResult> RestoreSessionAsync(CancellationToken cancellationToken = default)
    {
        var existing = await store.ReadAsync();
        if (existing is null) return new AuthSessionRestoreResult(AuthSessionRestoreState.SignedOut, null, null);
        if (!IsExpired(existing)) return new AuthSessionRestoreResult(AuthSessionRestoreState.SignedIn, existing, null);

        var refreshToken = existing.RefreshToken;
        var role = existing.Role ?? "student";
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            store.Clear();
            return new AuthSessionRestoreResult(AuthSessionRestoreState.ExpiredNoRefreshToken, null, null);
        }

        var refreshed = await authProvider.RefreshAsync(refreshToken, cancellationToken);
        if (!refreshed.IsSuccess)
        {
            store.Clear();
            return new AuthSessionRestoreResult(AuthSessionRestoreState.RefreshFailed, null,
                refreshed.Error ?? "Neuspješno ažuriranje tokena");
        }

        if (string.IsNullOrWhiteSpace(refreshed.AccessToken))
        {
            store.Clear();
            return new AuthSessionRestoreResult(AuthSessionRestoreState.RefreshFailed, null,
                "Neuspješno ažuriranje, access token nedostaje");
        }

        var refreshTokenToSave =
            string.IsNullOrWhiteSpace(refreshed.RefreshToken) ? refreshToken : refreshed.RefreshToken;
        var newToken = new Token(refreshed.AccessToken, refreshTokenToSave, refreshed.ExpiresAt,role);
        await store.SaveAsync(newToken);

        return new AuthSessionRestoreResult(AuthSessionRestoreState.Refreshed, newToken, null);
    }

    public async Task<TokenGrantResult> LoginAsync(CancellationToken token = default)
    {
        var result = await authProvider.LoginAsync(token);
        if (result.IsSuccess && !string.IsNullOrWhiteSpace(result.AccessToken))
        {
            await store.SaveAsync(new Token(result.AccessToken, result.RefreshToken, result.ExpiresAt, result.Role));
        }

        return result;
    }

    public async Task LogoutAsync(CancellationToken ct = default)
    {
        try
        {
            await authProvider.LogoutAsync(ct);
        }
        finally
        {
            store.Clear();
        }
    }

    private bool IsExpired(Token token)
    {
        if (token.ExpiresAt is null) return false;
        return token.ExpiresAt.Value <= DateTimeOffset.UtcNow.Add(expiredGracePeriod);
    }
}