using System.Security.Claims;
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
                return new TokenGrantResult(false, null, null, null, result.Error, null, null, null);

            const string roleNamespace = "https://campus4u.foi.hr/claims/role";
            var role = result.User?.FindFirst(roleNamespace)?.Value ?? "student";
            var sub = result?.User?.FindFirst("sub")?.Value;
            var email = result?.User?.FindFirst("email")?.Value ?? result?.User?.FindFirst(ClaimTypes.Email)?.Value;
            return new TokenGrantResult(true, result.AccessToken, result.RefreshToken, result.AccessTokenExpiration,
                null, role, sub, email);
        }
        catch (OperationCanceledException)
        {
            return new TokenGrantResult(false, null, null, null, "Prijava prekinuta.", null, null, null);
        }
        catch (Exception ex)
        {
            return new TokenGrantResult(false, null, null, null, ex.Message, null, null, null);
        }
    }

    public async Task<TokenGrantResult> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return new TokenGrantResult(false, null, null, null, "Refresh token ne postoji", null, null, null);

        try
        {
            var r = await client.RefreshTokenAsync(refreshToken, cancellationToken: ct);
            if (r.IsError) return new TokenGrantResult(false, null, null, null, r.Error, null, null, null);
            if (string.IsNullOrWhiteSpace(r.AccessToken))
                return new TokenGrantResult(false, null, null, null, "Access token ne postoji", null, null, null);

            return new TokenGrantResult(true, r.AccessToken, r.RefreshToken, r.AccessTokenExpiration, null, null, null,
                null);
        }
        catch (OperationCanceledException)
        {
            return new TokenGrantResult(false, null, null, null, "Refresh prekinut", null, null, null);
        }
        catch (Exception ex)
        {
            return new TokenGrantResult(false, null, null, null, ex.Message, null, null, null);
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