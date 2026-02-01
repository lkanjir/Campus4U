using System.Net;
using System.Net.Http.Headers;
using Client.Application.Auth;
using Client.Application.Images;

namespace Client.Data.Images;

public sealed class ImageApiClient : IImageSource
{
    private readonly HttpClient http;
    private readonly ITokenStore tokenStore;

    public ImageApiClient(string baseUrl, ITokenStore tokenStore, HttpClient? httpClient = null)
    {
        if (string.IsNullOrWhiteSpace(baseUrl)) throw new ArgumentException("BaseUrl nije definiran", nameof(baseUrl));

        this.tokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
        http = httpClient ?? new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    public async Task<ImagePayload?> GetProfileImageAsync(int userId, CancellationToken ct = default)
    {
        if (userId <= 0) return null;

        var token = await tokenStore.ReadAsync();
        if (string.IsNullOrWhiteSpace(token?.AccessToken)) return null;

        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/images/profiles/{userId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

        using var response = await http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
        if (response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            return null;

        response.EnsureSuccessStatusCode();
        var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
        var bytes = await response.Content.ReadAsByteArrayAsync(ct);
        return new ImagePayload(bytes, contentType);
    }

    public async Task UploadProfileImageAsync(ImageUpload upload, CancellationToken ct = default)
    {
        var token = await tokenStore.ReadAsync();
        if (string.IsNullOrWhiteSpace(token?.AccessToken)) throw new InvalidOperationException("Nema access tokena");

        if (upload.Content.CanSeek) upload.Content.Position = 0;
        using var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(upload.Content);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(upload.ContentType);
        content.Add(fileContent, "file", upload.FileName);

        using var request = new HttpRequestMessage(HttpMethod.Post, "api/images/profiles");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        request.Content = content;
        
        using var response = await http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
        response.EnsureSuccessStatusCode();
    }
}