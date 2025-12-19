using Auth0.OidcClient;
using Client.Domain.Auth;
using Duende.IdentityModel.OidcClient;
using Duende.IdentityModel.OidcClient.Browser;

namespace Client.Presentation.Auth;

//Luka Kanjir
public sealed class AuthService
{
    private readonly Auth0Client client;
    private readonly SecureTokenStore store;
    private readonly TimeSpan expiredGracePeriod = TimeSpan.FromMinutes(1);

    public AuthService(AuthOptions options, SecureTokenStore store)
    {
        var clientOptions = new Auth0ClientOptions
        {
            Domain = options.Domain,
            ClientId = options.ClientId,
            RedirectUri = options.RedirectUri,
            PostLogoutRedirectUri = options.PostLogoutRedirectUri,
            Browser = new SystemBrowser(),
            Scope = options.Scope,
        };

        client = new Auth0Client(clientOptions);
        this.store = store;
    }

    public async Task<AuthSessionRestoreResult> RestoreSessionAsync(CancellationToken cancellationToken = default)
    {
        var existing = await store.ReadAsync();
        if (existing is null) return new AuthSessionRestoreResult(AuthSessionRestoreState.SignedOut, null, null);
        if (!IsExpired(existing)) return new AuthSessionRestoreResult(AuthSessionRestoreState.SignedIn, existing, null);

        var refreshToken = existing.RefreshToken;
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            store.Clear();
            return new AuthSessionRestoreResult(AuthSessionRestoreState.ExpiredNoRefreshToken, null, null);
        }

        var refreshed = await client.RefreshTokenAsync(refreshToken, cancellationToken);
        if (refreshed.IsError)
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
        var newToken = new Token(refreshed.AccessToken, refreshTokenToSave, refreshed.AccessTokenExpiration);
        await store.SaveAsync(newToken);

        return new AuthSessionRestoreResult(AuthSessionRestoreState.Refreshed, newToken, null);
    }

    public async Task<LoginResult> LoginAsync(CancellationToken token = default)
    {
        //samo za test
        var extraParameters = new Dictionary<string, string>();
        extraParameters.Add("audience", "https://test-api.local");
        //end test

        var result = await client.LoginAsync(cancellationToken: token, extraParameters: extraParameters);
        if (!result.IsError)
        {
            await store.SaveAsync(new Token(result.AccessToken, result.RefreshToken, result.AccessTokenExpiration));
        }

        return result;
    }

    public async Task<BrowserResultType> LogoutAsync(bool federated = false, CancellationToken token = default)
    {
        try
        {
            return await client.LogoutAsync(federated, null, token);
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

    public void ClearLocalSession() => store.Clear();
}