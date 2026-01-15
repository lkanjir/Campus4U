using Client.Application.Fault;
using Client.Data.Context;
using Client.Data.Entities;
using Client.Domain.Fault;
using Microsoft.EntityFrameworkCore;

// Tin Posavec

namespace Client.Data.Fault
{
    public sealed class FaultRepository : IFaultRepository
    {
        public async Task<List<FaultType>> DohvatiVrsteKvarova()
        {
            await using var db = new Campus4UContext();

            return await db.VrsteKvarova
                .Select(v => new FaultType(v.VrstaKvaraId, v.Naziv))
                .ToListAsync();
        }

        public async Task<bool> SpremiKvar(FaultReport kvar)
        {
            await using var db = new Campus4UContext();

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
            await db.SaveChangesAsync();

            return true;
        }
    }
}
