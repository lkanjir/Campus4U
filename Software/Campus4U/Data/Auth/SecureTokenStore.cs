using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Client.Application.Auth;
using Client.Domain.Auth;

namespace Client.Data.Auth;

//Luka Kanjir
public sealed class SecureTokenStore : ITokenStore
{
    private readonly string path =
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Campus4U",
            "auth.dat"
        );

    public async Task SaveAsync(Token token)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directory)) Directory.CreateDirectory(directory);

        var json = JsonSerializer.Serialize(token);
        var bytes = ProtectedData.Protect(Encoding.UTF8.GetBytes(json), null, DataProtectionScope.CurrentUser);
        await File.WriteAllBytesAsync(path, bytes);
    }

    public async Task<Token?> ReadAsync()
    {
        if (!File.Exists(path)) return null;
        var bytes = await File.ReadAllBytesAsync(path);
        if (bytes.Length == 0)
        {
            Clear();
            return null;
        }

        try
        {
            var data = ProtectedData.Unprotect(bytes, null, DataProtectionScope.CurrentUser);
            return JsonSerializer.Deserialize<Token>(Encoding.UTF8.GetString(data));
        }
        catch (CryptographicException)
        {
            Clear();
            return null;
        }
        catch (JsonException)
        {
            Clear();
            return null;
        }
        
    }

    public void Clear()
    {
        if (File.Exists(path)) File.Delete(path);
    }
}