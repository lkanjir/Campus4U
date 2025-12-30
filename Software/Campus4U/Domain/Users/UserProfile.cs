namespace Client.Domain.Users;

//Luka Kanjir
public sealed record UserProfile(string Sub, string Email, string? Ime, string? Prezime, string? BrojSobe, int UlogaId)
{
    public bool IsOnboardingComplete => !string.IsNullOrWhiteSpace(Ime) && !string.IsNullOrWhiteSpace(Prezime) &&
                                        !string.IsNullOrWhiteSpace(Email);
}