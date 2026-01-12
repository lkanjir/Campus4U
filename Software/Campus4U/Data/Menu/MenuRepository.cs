using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Application.Menu;
using Client.Data.Context;
using Client.Data.Entities;
using Client.Domain.Menu;
using Microsoft.EntityFrameworkCore;

namespace Client.Data.Menu
{
    public sealed class MenuRepository : IMenuRepository
    {
        public async Task<IEnumerable<DailyMenu>> DohvatiSveJelovnike()
        {
            await using var db = new Campus4UContext();
            var jelovnici = await db.DnevniJelovnik
                .Include(j => j.Jela)
                .ToListAsync();

            return jelovnici.Select(j => new DailyMenu(
                j.JelovnikId,
                j.Datum,
                j.DanUTjednu,
                j.ZadnjeAzurirano,
                j.Jela.Select(x => new Meal(x.JeloId, x.JelovnikId, x.Naziv, x.Kategorija)).ToList()
            ));
        }

        public async Task<DailyMenu?> DohvatiJelovnikZaDatum(DateTime datum)
        {
            await using var db = new Campus4UContext();
            var j = await db.DnevniJelovnik
                .Include(j => j.Jela)
                .FirstOrDefaultAsync(j => j.Datum.Date == datum.Date);

            if (j == null) 
                return null;

            return new DailyMenu(
                j.JelovnikId,
                j.Datum,
                j.DanUTjednu,
                j.ZadnjeAzurirano,
                j.Jela.Select(x => new Meal(x.JeloId, x.JelovnikId, x.Naziv, x.Kategorija)).ToList()
            );
        }

        public async Task<IEnumerable<DailyMenu>> DohvatiTjedniJelovnik(DateTime pocetakTjedna)
        {
            await using var db = new Campus4UContext();

            var krajTjedna = pocetakTjedna.AddDays(7);

            var jelovnici = await db.DnevniJelovnik
                .Include(j => j.Jela)
                .Where(j => j.Datum >= pocetakTjedna && j.Datum < krajTjedna)
                .OrderBy(j => j.Datum)
                .ToListAsync();

            return jelovnici.Select(j => new DailyMenu(
                j.JelovnikId,
                j.Datum,
                j.DanUTjednu,
                j.ZadnjeAzurirano,
                j.Jela.Select(x => new Meal(x.JeloId, x.JelovnikId, x.Naziv, x.Kategorija)).ToList()
            ));
        }

        public bool SpremiJelovnik(DailyMenu jelovnik)
        {
            try
            {
                using var db = new Campus4UContext();

                var postojeci = db.DnevniJelovnik
                    .FirstOrDefault(j => j.Datum.Date == jelovnik.Datum.Date);

                if (postojeci != null)
                {
                    db.Jelo.RemoveRange(db.Jelo.Where(j => j.JelovnikId == postojeci.JelovnikId));
                    postojeci.DanUTjednu = jelovnik.DanUTjednu;
                    postojeci.ZadnjeAzurirano = DateTime.Now;

                    foreach (var jelo in jelovnik.Jela)
                    {
                        db.Jelo.Add(new Jelo
                        {
                            JelovnikId = postojeci.JelovnikId,
                            Naziv = jelo.Naziv,
                            Kategorija = jelo.Kategorija
                        });
                    }
                }
                else
                {
                    var novi = new DnevniJelovnik
                    {
                        Datum = jelovnik.Datum,
                        DanUTjednu = jelovnik.DanUTjednu,
                        ZadnjeAzurirano = DateTime.Now
                    };
                    db.DnevniJelovnik.Add(novi);
                    db.SaveChanges();

                    foreach (var jelo in jelovnik.Jela)
                    {
                        db.Jelo.Add(new Jelo
                        {
                            JelovnikId = novi.JelovnikId,
                            Naziv = jelo.Naziv,
                            Kategorija = jelo.Kategorija
                        });
                    }
                }

                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SpremiViseJelovnika(IEnumerable<DailyMenu> jelovnici)
        {
            foreach (var jelovnik in jelovnici)
            {
                if (!SpremiJelovnik(jelovnik))
                    return false;
            }
            return true;
        }

        public bool ObrisiStareJelovnike(DateTime datum)
        {
            try
            {
                using var db = new Campus4UContext();
                var stari = db.DnevniJelovnik.Where(j => j.Datum < datum);
                db.DnevniJelovnik.RemoveRange(stari);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<DateTime?> DohvatiDatumZadnjegAzuriranja()
        {
            await using var db = new Campus4UContext();
            return await db.DnevniJelovnik
                .MaxAsync(j => (DateTime?)j.ZadnjeAzurirano);
        }

        public bool IsJelovnikAzuriran(DateTime datum)
        {
            using var db = new Campus4UContext();
            var jelovnik = db.DnevniJelovnik
                .FirstOrDefault(j => j.Datum.Date == datum.Date);

            if (jelovnik == null) 
                return false;
            return jelovnik.ZadnjeAzurirano.Date == DateTime.Today;
        }
    }
}