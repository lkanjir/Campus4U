namespace Api.Configuration;

public sealed class Auth0Options
{
    public string? Domain { get; init; }
    public string? Audience { get; init; }
}