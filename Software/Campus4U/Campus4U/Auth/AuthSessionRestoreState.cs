namespace Client.Presentation.Auth;

//Luka Kanjir
public enum AuthSessionRestoreState
{
    SignedOut,
    SignedIn,
    ExpiredNoRefreshToken,
    Refreshed,
    RefreshFailed
}

//Luka Kanjir
public sealed record AuthSessionRestoreResult(
    AuthSessionRestoreState State,
    Token? Token,
    string? Error
);