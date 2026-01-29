//Marko Mišić

namespace Client.Domain.Spaces
{
    public sealed record Rezervacija(
        int ID,
        Space Prostor,
        int KorisnikId,
        DateTime PocetnoVrijeme,
        DateTime KrajnjeVrijeme,
        string Status
    );
}
