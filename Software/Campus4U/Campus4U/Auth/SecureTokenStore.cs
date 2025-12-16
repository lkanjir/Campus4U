using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Client.Presentation.Auth;

public sealed record Token(
    string AccessToken,
    string? RefreshToken,
    DateTimeOffset? ExpiresAt
);

public sealed class SecureTokenStore
{
    private readonly string path =
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Campus4U",
            "auth.dat"
        );

    public async Task SaveAsync(Token token)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        var json = JsonSerializer.Serialize(token);
        var bytes = ProtectedData.Protect(Encoding.UTF8.GetBytes(json), null, DataProtectionScope.CurrentUser);
        await File.WriteAllBytesAsync(path, bytes);
    }

    public async Task<Token> ReadAsync()
    {
        if(!File.Exists(path)) return null;
        var bytes = await File.ReadAllBytesAsync(path);
        var data = ProtectedData.Unprotect(bytes, null, DataProtectionScope.CurrentUser);
        return JsonSerializer.Deserialize<Token>(Encoding.UTF8.GetString(data));
    }

    public void Clear()
    {
        if(File.Exists(path)) File.Delete((path));
    }
}