using Client.Domain.Users;

namespace Client.Application.Users;

//Luka Kanjir
public interface IUserProfileRepository
{
    Task<UserProfile?> GetBySubAsync(string sub, CancellationToken ct = default);
    Task<int?> GetRoleIdByNameAsync(string roleName, CancellationToken ct = default);
    Task SaveAsync(UserProfile profile, CancellationToken ct = default);
    Task<bool> AzurirajKorisnika(UserProfile profile);
    Task<bool> AzurirajProfilnuSliku(int id, string urlSlike);
}
