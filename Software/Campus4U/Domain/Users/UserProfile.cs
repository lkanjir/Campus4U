namespace Client.Domain.Users;

//Luka Kanjir
public sealed record UserProfile(int Id, string Sub, string Email, string? Ime, string? Prezime, string? KorisnickoIme, string? BrojSobe, string? BrojTelefona, string? SlikaProfila, int UlogaId)
{
    public bool IsOnboardingComplete => !string.IsNullOrWhiteSpace(Ime) && !string.IsNullOrWhiteSpace(Prezime) &&
                                        !string.IsNullOrWhiteSpace(Email);
}