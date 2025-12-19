namespace Client.Application.Auth;

//Luka Kanjir
public interface IAuthProvider
{
    Task<TokenGrantResult> LoginAsync(CancellationToken ct = default);
    Task<TokenGrantResult> RefreshAsync(string refreshToken, CancellationToken ct = default);
    Task LogoutAsync(CancellationToken ct = default);
}
