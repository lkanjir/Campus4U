namespace Client.Application.Auth;

//Luka Kanjir
public sealed class AuthOptions
{
    public required string Domain { get; init; }
    public required string ClientId { get; init; }
    public required string Scope { get; init; }
    public required string RedirectUri { get; init; }
    public required string PipeName { get; init; }
}