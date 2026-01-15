using Client.Application.Fault;
using Client.Data.Context;
using Client.Data.Entities;
using Client.Domain.Fault;

// Tin Posavec
namespace Client.Data.Fault
{
    public sealed class FaultRepository : IFaultRepository
    {
        public List<FaultType> DohvatiVrsteKvarova()
        {
            using var db = new Campus4UContext();

            var kvarovi = db.VrsteKvarova
                .Select(v => new FaultType(v.VrstaKvaraId, v.Naziv))
                .ToList();

            return kvarovi;
        }

        public bool SpremiKvar(FaultReport kvar)
        {
            using var db = new Campus4UContext();

            var entitet = new Kvarovi
            {
                KorisnikId = kvar.KorisnikId,
                ProstorId = kvar.ProstorId,
                VrstaKvaraId = kvar.VrstaKvaraId,
                Opis = kvar.Opis,
                Fotografija = kvar.Fotografija,
                Status = "Aktivan",
                DatumPrijave = DateTime.Now
            };

            db.Kvarovi.Add(entitet);
            db.SaveChanges();

            return true;
        }
    }
}
