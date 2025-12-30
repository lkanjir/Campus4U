namespace Client.Domain.Auth;

//Luka Kanjir
public sealed record AuthSessionRestoreResult(
    AuthSessionRestoreState State,
    Token? Token,
    string? Error
);