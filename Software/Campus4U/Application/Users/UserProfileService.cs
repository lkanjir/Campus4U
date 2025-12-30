using Client.Domain.Users;

namespace Client.Application.Users;

//Luka Kanjir
public sealed record SaveUserProfileResult(bool isSuccess, string? Error);

//Luka Kanjir
public class UserProfileService(IUserRepository repository)
{
    public Task<UserProfile?> GetBySubAsync(string sub, CancellationToken ct = default) =>
        string.IsNullOrWhiteSpace(sub) ? Task.FromResult<UserProfile?>(null) : repository.GetBySubAsync(sub, ct);

    public async Task<SaveUserProfileResult> SaveAsync(string sub, string email, string? ime, string? prezime,
        string? brojSobe, string roleName, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(sub) || string.IsNullOrWhiteSpace(email))
            return new SaveUserProfileResult(false, "Nedostaje sub ili email");

        var requestedRole = string.IsNullOrWhiteSpace(roleName) ? "student" : roleName;
        var role = await repository.GetRoleIdByNameAsync(requestedRole, ct) ??
                   await repository.GetRoleIdByNameAsync("student", ct);

        if (role is null) return new SaveUserProfileResult(false, "Uloga nije pronađena u bazi");

        var profile = new UserProfile(sub.Trim(), email.Trim(), string.IsNullOrWhiteSpace(ime) ? null : ime.Trim(),
            string.IsNullOrWhiteSpace(prezime) ? null : prezime.Trim(),
            string.IsNullOrWhiteSpace(brojSobe) ? null : brojSobe.Trim(), role.Value);

        await repository.SaveAsync(profile, ct);
        return new SaveUserProfileResult(true, null);
    }
}