using Auth0.OidcClient;
using Duende.IdentityModel.OidcClient;

namespace Client.Presentation.Auth;

//Luka Kanjir
public sealed class AuthService
{
    private readonly Auth0Client client;
    private readonly SecureTokenStore store;

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

    public async Task<LoginResult> LoginAsync(CancellationToken token = default)
    {
        var result = await client.LoginAsync(cancellationToken: token);
        if (!result.IsError)
        {
            await store.SaveAsync(new Token(result.AccessToken, result.RefreshToken, result.AccessTokenExpiration));
        }

        return result;
    }
}