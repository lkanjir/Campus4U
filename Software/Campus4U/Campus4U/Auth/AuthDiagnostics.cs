using System.Net.Http;
using System.Net.Http.Headers;

namespace Client.Presentation.Auth;

//Luka Kanjir
public sealed record DebugRefreshResult(
    AuthSessionRestoreState RestoreState,
    bool IsSuccess,
    DateTimeOffset? ExpiresAt,
    string? Payload,
    string? Error
);

//Luka Kanjir
public class AuthDiagnostics(AuthService auth, string domain)
{
    private readonly AuthService auth = auth;
    private readonly HttpClient http = new();
    private readonly string domain = domain;

    public async Task<DebugRefreshResult> RunAsync(CancellationToken cancellationToken = default)
    {
        var restore = await auth.RestoreSessionAsync(cancellationToken);
        if (restore.State is not AuthSessionRestoreState.SignedIn and not AuthSessionRestoreState.Refreshed)
            return new DebugRefreshResult(restore.State, false, null, null, restore.Error);

        var access = restore.Token?.AccessToken;
        if (string.IsNullOrWhiteSpace(access))
            return new DebugRefreshResult(restore.State, false, null, null, "Nema access tokena nakon restore");

        var expiresAt = restore.Token?.ExpiresAt;
        using var req = new HttpRequestMessage(HttpMethod.Get, $"https://{domain}/userinfo");
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access);

        var res = await http.SendAsync(req, cancellationToken);
        var body = await res.Content.ReadAsStringAsync(cancellationToken);

        return res.IsSuccessStatusCode
            ? new DebugRefreshResult(restore.State, true, expiresAt, body, null)
            : new DebugRefreshResult(restore.State, false, expiresAt, body, $"HTTP {(int)res.StatusCode}");
    }
}