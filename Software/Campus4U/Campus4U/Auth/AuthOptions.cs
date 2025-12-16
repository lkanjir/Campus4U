using Duende.IdentityModel;

namespace Client.Presentation.Auth;

//Luka Kanjir
public sealed class AuthOptions
{
    public required string Domain { get; init; }
    public required string ClientId { get; init; }
    public required string RedirectUrl { get; init; }
}