using Client.Domain.Menu;
using Client.Domain.Users;

namespace Client.Application.Users;

//Luka Kanjir
public sealed record SaveUserProfileResult(bool isSuccess, string? Error);

//Luka Kanjir
public class UserProfileService(IUserProfileRepository profileRepository)
{
    public Task<UserProfile?> GetBySubAsync(string sub, CancellationToken ct = default) =>
        string.IsNullOrWhiteSpace(sub) ? Task.FromResult<UserProfile?>(null) : profileRepository.GetBySubAsync(sub, ct);

    public async Task<SaveUserProfileResult> SaveAsync(string sub, string email, string? ime, string? prezime,
        string? brojSobe, string? brojTelefona, string? slikaProfila, string korIme, string roleName, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(sub) || string.IsNullOrWhiteSpace(email))
            return new SaveUserProfileResult(false, "Nedostaje sub ili email");

        var requestedRole = string.IsNullOrWhiteSpace(roleName) ? "student" : roleName;
        var role = await profileRepository.GetRoleIdByNameAsync(requestedRole, ct) ??
                   await profileRepository.GetRoleIdByNameAsync("student", ct);

        if (role is null) return new SaveUserProfileResult(false, "Uloga nije pronađena u bazi");

        var profile = new UserProfile(0, sub.Trim(), email.Trim(), string.IsNullOrWhiteSpace(ime) ? null : ime.Trim(),
            string.IsNullOrWhiteSpace(prezime) ? null : prezime.Trim(),
            string.IsNullOrWhiteSpace(korIme) ? null : korIme.Trim(),
            string.IsNullOrWhiteSpace(brojSobe) ? null : brojSobe.Trim(), 
            string.IsNullOrWhiteSpace(brojTelefona) ? null : brojTelefona.Trim(),
            string.IsNullOrWhiteSpace(slikaProfila) ? null : slikaProfila.Trim(), role.Value);

        await profileRepository.SaveAsync(profile, ct);
        return new SaveUserProfileResult(true, null);
    }

    public async Task<SaveUserProfileResult> AzurirajProfilAsync(UserProfile profile, string? ime, string? prezime, string? korisnickoIme, string? brojSobe, string? brojTelefona)
    {
        if (profile.Id <= 0)
            return new SaveUserProfileResult(false, "Neispravan ID korisnika");

        if (string.IsNullOrWhiteSpace(profile.Sub) || string.IsNullOrWhiteSpace(profile.Email))
            return new SaveUserProfileResult(false, "Nedostaje sub ili email");

        static string? NormalizeUpdate(string? input, string? current)
        {
            if (input is null)
                return current;

            var trimmed = input.Trim();
            return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
        }

        var azuriranProfile = profile with {
            Ime = NormalizeUpdate(ime, profile.Ime),
            Prezime = NormalizeUpdate(prezime, profile.Prezime),
            KorisnickoIme = NormalizeUpdate(korisnickoIme, profile.KorisnickoIme),
            BrojSobe = NormalizeUpdate(brojSobe, profile.BrojSobe),
            BrojTelefona = NormalizeUpdate(brojTelefona, profile.BrojTelefona)
        };
        bool stanje = await profileRepository.AzurirajKorisnikaAsync(azuriranProfile);
        if(!stanje) return new SaveUserProfileResult(false, "Neuspješna izmjena podataka korisnika");
        return new SaveUserProfileResult(true, null);
    }
}
