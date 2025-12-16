namespace Client.Presentation.Auth;

//Luka Kanjir
public sealed record AuthOptions(
    string Domain,
    string ClientId,
    string RedirectUrl
);