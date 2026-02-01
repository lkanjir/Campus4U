using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Client.Application.Auth;

namespace Client.Data.Auth;

//Luka Kanjir
public sealed class Auth0AccountDeletionService : IAccountDeletionService
{
    private readonly Auth0ManagementOptions options;
    private readonly HttpClient http;

    public Auth0AccountDeletionService(Auth0ManagementOptions options, HttpClient? httpClient = null)
    {
        this.options = options;
        http = httpClient ?? new HttpClient();
    }

    public async Task<AccountDeletionResult> DeleteAccountAsync(string userId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return new AccountDeletionResult(false, "UserId nije naveden.");
        }

        try
        {
            var token = await GetManagementTokenAsync(ct);
            var domain = options.Domain.TrimEnd('/');
            var url = $"{domain}/api/v2/users/{Uri.EscapeDataString(userId)}";

            using var req = new HttpRequestMessage(HttpMethod.Delete, url);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var res = await http.SendAsync(req, ct);
            if (res.IsSuccessStatusCode)
            {
                return new AccountDeletionResult(true, null);
            }

            var body = await res.Content.ReadAsStringAsync(ct);
            var error = string.IsNullOrWhiteSpace(body) ? "Brisanje nije uspjelo." : body;
            return new AccountDeletionResult(false, error);
        }
        catch (TaskCanceledException ex)
        {
            return new AccountDeletionResult(false, ex.Message);
        }
        catch (HttpRequestException ex)
        {
            return new AccountDeletionResult(false, ex.Message);
        }
    }

    private async Task<string> GetManagementTokenAsync(CancellationToken ct)
    {
        var domain = options.Domain.TrimEnd('/');
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_id", options.ClientId),
            new KeyValuePair<string, string>("client_secret", options.ClientSecret),
            new KeyValuePair<string, string>("audience", $"{domain}/api/v2/")
        });

        var res = await http.PostAsync($"{domain}/oauth/token", content, ct);
        res.EnsureSuccessStatusCode();

        var json = await res.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("access_token").GetString()!;
    }
}
