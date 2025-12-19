using Client.Application.Auth;
using Duende.IdentityModel.Client;
using Duende.IdentityModel.OidcClient;
using Duende.IdentityModel.OidcClient.Browser;

namespace Client.Data.Auth;

//Luka Kanjir
public sealed class OidcProvider : IAuthProvider
{
    private readonly OidcClient client;
    private readonly AuthOptions options;

    public OidcProvider(AuthOptions options, IBrowser browser)
    {
        this.options = options;
        var oidcOptions = new OidcClientOptions
        {
            Authority = options.Domain,
            ClientId = options.ClientId,
            RedirectUri = options.RedirectUri,
            PostLogoutRedirectUri = options.PostLogoutRedirectUri,
            Scope = options.Scope,
            Browser = browser
        };
        client = new OidcClient(oidcOptions);
    }


    public async Task<TokenGrantResult> LoginAsync(CancellationToken ct = default)
    {
        try
        {
            var loginRequest = new LoginRequest
            {
                FrontChannelExtraParameters = string.IsNullOrWhiteSpace(options.Audience)
                    ? null
                    : new Parameters
                    {
                        { "audience", options.Audience }
                    }
            };

            var result = await client.LoginAsync(loginRequest, ct);
            if (result.IsError)
                return new TokenGrantResult(false, null, null, null, result.Error);

            return new TokenGrantResult(true, result.AccessToken, result.RefreshToken, result.AccessTokenExpiration,
                null);
        }
        catch (OperationCanceledException)
        {
            return new TokenGrantResult(false, null, null, null, "Prijava prekinuta.");
        }
        catch (Exception ex)
        {
            return new TokenGrantResult(false, null, null, null, ex.Message);
        }
    }

    public async Task<TokenGrantResult> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return new TokenGrantResult(false, null, null, null, "Refresh token ne postoji");

        try
        {
            var r = await client.RefreshTokenAsync(refreshToken, cancellationToken: ct);
            if (r.IsError) return new TokenGrantResult(false, null, null, null, r.Error);
            if (string.IsNullOrWhiteSpace(r.AccessToken))
                return new TokenGrantResult(false, null, null, null, "Access token ne postoji");

            return new TokenGrantResult(true, r.AccessToken, r.RefreshToken, r.AccessTokenExpiration, null);
        }
        catch (OperationCanceledException)
        {
            return new TokenGrantResult(false, null, null, null, "Refresh prekinut");
        }
        catch (Exception ex)
        {
            return new TokenGrantResult(false, null, null, null, ex.Message);
        }
    }

    public async Task LogoutAsync(CancellationToken ct = default)
    {
        var authority = options.Domain;
        var clientId = Uri.EscapeDataString(options.ClientId);
        var returnTo = Uri.EscapeDataString(options.PostLogoutRedirectUri);
        var url = $"{authority}/v2/logout?client_id={clientId}&returnTo={returnTo}";
        var browserOptions = new BrowserOptions(url, options.PostLogoutRedirectUri)
        {
            Timeout = TimeSpan.FromSeconds(120),
        };
        await client.Options.Browser.InvokeAsync(browserOptions, ct);
    }
}