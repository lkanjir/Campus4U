namespace Client.Domain.Auth;

//Luka Kanjir
public sealed record Token(
    string AccessToken,
    string? RefreshToken,
    DateTimeOffset? ExpiresAt,
    string? Role
);
