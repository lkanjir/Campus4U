
// Tin Posavec, Marko Mišić

namespace Client.Domain.Spaces
{
    public sealed record Space(
        int ProstorId,
        string Naziv,
        int Kapacitet,
        string Oprema,
        string Opis,
        Dom Dom,
        TipProstora TipProstora,
        string? SlikaPutanja
        );
    
}
