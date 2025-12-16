using Auth0.OidcClient;
using Duende.IdentityModel.OidcClient;

namespace Client.Presentation.Auth;

//Luka Kanjir
public sealed class AuthService
{
    private readonly Auth0Client client;

    public AuthService(AuthOptions options)
    {
        client = new Auth0Client(new Auth0ClientOptions
        {
            Domain = options.Domain,
            ClientId = options.ClientId,
            RedirectUri = options.RedirectUrl,
        });
    }

    public Task<LoginResult> LoginAsync(CancellationToken token = default)
    {
        return client.LoginAsync(cancellationToken: token);
    }
}