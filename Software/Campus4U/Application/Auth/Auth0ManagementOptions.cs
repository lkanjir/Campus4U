namespace Client.Application.Auth;

public sealed record Auth0ManagementOptions
{
    public required string Domain { get; init; }
    public required string ClientId { get; init; }
    public required string ClientSecret { get; init; }
}
