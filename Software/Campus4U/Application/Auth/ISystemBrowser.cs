namespace Client.Application.Auth;

public sealed record SystemBrowserResult(bool IsSuccess, string? ResponseUrl, string? Error);

public interface ISystemBrowser
{
    Task<SystemBrowserResult> OpenAsync(string startUrl, string endUrl, CancellationToken ct = default);
}