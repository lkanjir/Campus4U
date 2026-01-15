
//Tin Posavec

namespace Client.Domain.Fault
{
    public sealed record FaultReport(
        int KvarId,
        int KorisnikId,
        int ProstorId,
        int VrstaKvaraId,
        string Opis,
        byte[]? Fotografija,
        string Status,
        DateTime DatumPrijave,
        string? ProstorNaziv = null,
        string? VrstaKvaraNaziv = null,
        string? KorisnikImePrezime = null
        );
   
}
