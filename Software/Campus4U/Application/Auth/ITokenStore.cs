using Client.Domain.Auth;

namespace Client.Application.Auth;

public interface ITokenStore
{
    Task SaveAsync(Token token);
    Task<Token?> ReadAsync();
    void Clear();
}