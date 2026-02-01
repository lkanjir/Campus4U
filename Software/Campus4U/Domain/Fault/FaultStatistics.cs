
//Tin Posavec

namespace Client.Domain.Fault
{
    public sealed record FaultStatistics(
        int UkupnoKvarova,
        int AktivnihKvarova,
        int UObradiKvarova,
        int RijesenihKvarova,
        List<StatistikaPoKategoriji> PoVrstiKvara,
        List<StatistikaPoKategoriji> PoProstoru,
        List<StatistikaPoMjesecu> PoMjesecu
    );

    public sealed record StatistikaPoKategoriji(
        string Naziv,
        int BrojKvarova,
        double Postotak
    );

    public sealed record StatistikaPoMjesecu(
        int Godina,
        int Mjesec,
        string NazivMjeseca,
        int BrojKvarova
    );
}
