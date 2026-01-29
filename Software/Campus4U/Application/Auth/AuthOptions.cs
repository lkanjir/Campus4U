namespace Client.Application.Auth;

//Luka Kanjir
public sealed record AuthOptions
{
    public required string Domain { get; init; }
    public required string ClientId { get; init; }
    public required string RedirectUri { get; init; }
    public required string PostLogoutRedirectUri { get; init; }
    public required string Scope { get; init; }
    public required string Audience { get; init; }
    public required string Connection { get; init; }
}