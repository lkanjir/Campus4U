using Client.Domain.Auth;

namespace Client.Application.Auth;

//Luka Kanjir
public interface ITokenStore
{
    Task SaveAsync(Token token);
    Task<Token?> ReadAsync();
    void Clear();
}