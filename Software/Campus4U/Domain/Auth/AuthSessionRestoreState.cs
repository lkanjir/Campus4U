namespace Client.Domain.Auth;

//Luka Kanjir
public enum AuthSessionRestoreState
{
    SignedOut,
    SignedIn,
    ExpiredNoRefreshToken,
    Refreshed,
    RefreshFailed
}