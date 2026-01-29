using System.Net.Http;
using Client.Application.Auth;

namespace Client.Data.Auth;

public sealed class Auth0PasswordResetService : IPasswordResetService
{
    private readonly AuthOptions options;
    private readonly HttpClient http;

    public Auth0PasswordResetService(AuthOptions options, HttpClient? httpClient = null)
    {
        this.options = options;
        http = httpClient ?? new HttpClient();
    }

    public async Task<PasswordResetResult> SendPasswordResetEmailAsync(string email, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return new PasswordResetResult(false, "Email nije naveden.");
        }

        var domain = options.Domain.TrimEnd('/');
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", options.ClientId),
            new KeyValuePair<string, string>("email", email.Trim()),
            new KeyValuePair<string, string>("connection", options.Connection)
        });

        try
        {
            var res = await http.PostAsync($"{domain}/dbconnections/change_password", content, ct);
            if (res.IsSuccessStatusCode)
            {
                return new PasswordResetResult(true, null);
            }

            var body = await res.Content.ReadAsStringAsync(ct);
            var error = string.IsNullOrWhiteSpace(body) ? "Slanje emaila nije uspjelo." : body;
            return new PasswordResetResult(false, error);
        }
        catch (TaskCanceledException ex)
        {
            return new PasswordResetResult(false, ex.Message);
        }
        catch (HttpRequestException ex)
        {
            return new PasswordResetResult(false, ex.Message);
        }
    }
}