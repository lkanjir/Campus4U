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
                Status = kvar.Status,
                DatumPrijave = kvar.DatumPrijave
            };

            db.Kvarovi.Add(entitet);
            var spremljen = await db.SaveChangesAsync();

            return spremljen > 0;
        }

        // FZ-11
        public async Task<List<FaultReport>> DohvatiSveKvarove()
        {
            await using var db = new Campus4UContext();

            return await db.Kvarovi
                .Include(k => k.Prostor)
                .Include(k => k.VrstaKvara)
                .Include(k => k.Korisnik)
                .Select(k => new FaultReport(
                    k.KvarId,
                    k.KorisnikId,
                    k.ProstorId,
                    k.VrstaKvaraId,
                    k.Opis,
                    k.Fotografija,
                    k.Status,
                    k.DatumPrijave,
                    k.Prostor.Naziv,
                    k.VrstaKvara.Naziv,
                    (k.Korisnik.Ime + " " + k.Korisnik.Prezime).Trim()
                ))
                .OrderByDescending(k => k.DatumPrijave)
                .ToListAsync();
        }

        public async Task<FaultReport?> DohvatiKvarPoId(int kvarId)
        {
            await using var db = new Campus4UContext();

            var k = await db.Kvarovi
                .Include(k => k.Prostor)
                .Include(k => k.VrstaKvara)
                .Include(k => k.Korisnik)
                .FirstOrDefaultAsync(k => k.KvarId == kvarId);

            if (k == null) return null;

            return new FaultReport(
                k.KvarId,
                k.KorisnikId,
                k.ProstorId,
                k.VrstaKvaraId,
                k.Opis,
                k.Fotografija,
                k.Status,
                k.DatumPrijave,
                k.Prostor.Naziv,
                k.VrstaKvara.Naziv,
                (k.Korisnik.Ime + " " + k.Korisnik.Prezime).Trim()
            );
        }

        public async Task<List<FaultReport>> DohvatiKvaroveFiltrirano(string? status, int? prostorId, int? vrstaKvaraId, DateTime? odDatuma, DateTime? doDatuma)
        {
            await using var db = new Campus4UContext();

            var upit = db.Kvarovi
                .Include(k => k.Prostor)
                .Include(k => k.VrstaKvara)
                .Include(k => k.Korisnik)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                upit = upit.Where(k => k.Status == status);

            if (prostorId.HasValue)
                upit = upit.Where(k => k.ProstorId == prostorId.Value);

            if (vrstaKvaraId.HasValue)
                upit = upit.Where(k => k.VrstaKvaraId == vrstaKvaraId.Value);

            if (odDatuma.HasValue)
                upit = upit.Where(k => k.DatumPrijave >= odDatuma.Value);

            if (doDatuma.HasValue)
                upit = upit.Where(k => k.DatumPrijave <= doDatuma.Value);

            return await upit
                .Select(k => new FaultReport(
                    k.KvarId,
                    k.KorisnikId,
                    k.ProstorId,
                    k.VrstaKvaraId,
                    k.Opis,
                    k.Fotografija,
                    k.Status,
                    k.DatumPrijave,
                    k.Prostor.Naziv,
                    k.VrstaKvara.Naziv,
                    (k.Korisnik.Ime + " " + k.Korisnik.Prezime).Trim()
                ))
                .OrderByDescending(k => k.DatumPrijave)
                .ToListAsync();
        }

        public async Task<bool> AzurirajStatusKvara(int kvarId, string noviStatus)
        {
            await using var db = new Campus4UContext();

            var kvar = await db.Kvarovi.FindAsync(kvarId);
            if (kvar == null) return false;

            kvar.Status = noviStatus;
            var azurirano = await db.SaveChangesAsync();

            return azurirano > 0;
        }

        public async Task<FaultStatistics> DohvatiStatistiku()
        {
            await using var db = new Campus4UContext();

            var sviKvarovi = await db.Kvarovi
                .Include(k => k.Prostor)
                .Include(k => k.VrstaKvara)
                .ToListAsync();

            var ukupno = sviKvarovi.Count;
            var aktivnih = sviKvarovi.Count(k => k.Status == "Aktivan");
            var uObradi = sviKvarovi.Count(k => k.Status == "U obradi");
            var rijesenih = sviKvarovi.Count(k => k.Status == "Riješen");

            var poVrsti = sviKvarovi
                .GroupBy(k => k.VrstaKvara.Naziv)
                .Select(g => new StatistikaPoKategoriji(
                    g.Key,
                    g.Count(),
                    ukupno > 0 ? Math.Round((double)g.Count() / ukupno * 100, 1) : 0
                ))
                .OrderByDescending(s => s.BrojKvarova)
                .ToList();

            var poProstoru = sviKvarovi
                .GroupBy(k => k.Prostor.Naziv)
                .Select(g => new StatistikaPoKategoriji(
                    g.Key,
                    g.Count(),
                    ukupno > 0 ? Math.Round((double)g.Count() / ukupno * 100, 1) : 0
                ))
                .OrderByDescending(s => s.BrojKvarova)
                .ToList();

            var poMjesecu = sviKvarovi
                .GroupBy(k => new { k.DatumPrijave.Year, k.DatumPrijave.Month })
                .Select(g => new StatistikaPoMjesecu(
                    g.Key.Year,
                    g.Key.Month,
                    DohvatiNazivMjeseca(g.Key.Month),
                    g.Count()
                ))
                .OrderByDescending(s => s.Godina)
                .ThenByDescending(s => s.Mjesec)
                .ToList();

            return new FaultStatistics(
                ukupno,
                aktivnih,
                uObradi,
                rijesenih,
                poVrsti,
                poProstoru,
                poMjesecu
            );
        }

        private static string DohvatiNazivMjeseca(int mjesec)
        {
            return mjesec switch
            {
                1 => "Siječanj",
                2 => "Veljača",
                3 => "Ožujak",
                4 => "Travanj",
                5 => "Svibanj",
                6 => "Lipanj",
                7 => "Srpanj",
                8 => "Kolovoz",
                9 => "Rujan",
                10 => "Listopad",
                11 => "Studeni",
                12 => "Prosinac",
                _ => "Nepoznato"
            };
        }
    }
}
