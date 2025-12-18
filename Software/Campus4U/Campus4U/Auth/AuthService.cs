using Auth0.OidcClient;
using Duende.IdentityModel.OidcClient;
using Microsoft.IdentityModel.Tokens;

namespace Client.Presentation.Auth;

//Luka Kanjir
public sealed class AuthService
{
    private readonly Auth0Client client;
    private readonly SecureTokenStore store;
    private readonly TimeSpan expiredGracePeriod = TimeSpan.FromMinutes(1);

    public AuthService(AuthOptions options, SecureTokenStore store)
    {
        client = new Auth0Client(new Auth0ClientOptions
        {
            Domain = options.Domain,
            ClientId = options.ClientId,
            RedirectUri = options.RedirectUri,
            PostLogoutRedirectUri = options.PostLogoutRedirectUri,
            Browser = new SystemBrowser()
        });
        this.store = store;
    }

    public async Task<Token?> GetTokenOrClearAsync()
    {
        var token = await store.ReadAsync();
        if (token is null) return null;
        if (IsExpired(token))
        {
            store.Clear();
            return null;
        }
        
        return token;
    }
    
    public async Task<LoginResult> LoginAsync(CancellationToken token = default)
    {
        var result = await client.LoginAsync(cancellationToken: token);
        if (!result.IsError)
        {
            await store.SaveAsync(new Token(result.AccessToken, result.RefreshToken, result.AccessTokenExpiration));
        }

        return result;
    }

    private bool IsExpired(Token token)
    {
        if (token.ExpiresAt is null) return false;
        return token.ExpiresAt.Value <= DateTimeOffset.UtcNow.Add(expiredGracePeriod);
    }
    public void ClearLocalSession() => store.Clear();
}