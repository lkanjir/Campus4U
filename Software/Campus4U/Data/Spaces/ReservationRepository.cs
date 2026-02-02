using Client.Application.Spaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Client.Data.Context;
using Client.Domain.Spaces;
using Client.Data.Entities;

//Marko Mišić 

namespace Client.Data.Spaces
{
    public sealed class ReservationRepository : IReservationRepository
    {
        public async Task<bool> ProvjeriDostupnost(int prostorId, DateTime pocetnoVrijeme, DateTime krajnjeVrijeme)
        {
            await using var db = new Campus4UContext();
            var postojiPreklapanje = await db.Rezervacije
                .AnyAsync(r => r.ProstorId == prostorId &&
                               r.Status != "Otkazano" &&
                               pocetnoVrijeme < r.VrijemeDo &&
                               krajnjeVrijeme > r.VrijemeOd
                                );
            return !postojiPreklapanje;
        }

        public async Task SpremiRezervaciju(Rezervacija rezervacija)
        {
            await using var db = new Campus4UContext();

            var novaRezervacija = new Rezervacije
            {
                ProstorId = rezervacija.Prostor.ProstorId,
                KorisnikId = rezervacija.KorisnikId,
                VrijemeOd = rezervacija.PocetnoVrijeme,
                VrijemeDo = rezervacija.KrajnjeVrijeme,
                DatumKreiranja = DateTime.Now,
                Status = rezervacija.Status,
                BrojOsoba = rezervacija.BrojOsoba
            };

            db.Rezervacije.Add(novaRezervacija);
            await db.SaveChangesAsync();
        }

        public async Task<int> DohvatiZauzetoMjesta(int prostorId, DateTime pocetnoVrijeme, DateTime krajnjeVrijeme)
        {
            await using var db = new Campus4UContext();

            var zauzeto = await db.Rezervacije
                .Where(r => r.ProstorId == prostorId
                            && r.Status != "Otkazano"
                            && r.VrijemeOd < krajnjeVrijeme
                            && r.VrijemeDo > pocetnoVrijeme)
                .SumAsync(r => (int?)r.BrojOsoba) ?? 0;

            return zauzeto;
        }

        public async Task<int> DohvatiZauzetoMjestaBezRezervacije(int prostorId, DateTime pocetnoVrijeme, DateTime krajnjeVrijeme, int rezervacijaId)
        {
            await using var db = new Campus4UContext();

            var zauzeto = await db.Rezervacije
                .Where(r => r.ProstorId == prostorId
                            && r.Status != "Otkazano"
                            && r.Id != rezervacijaId
                            && r.VrijemeOd < krajnjeVrijeme
                            && r.VrijemeDo > pocetnoVrijeme)
                .SumAsync(r => (int?)r.BrojOsoba) ?? 0;

            return zauzeto;
        }


        public async Task<List<Rezervacija>> DohvatiRezervacijeKorisnika(int korisnikId)
        {
            await using var db = new Campus4UContext();
            var rezervacijeEntities = await db.Rezervacije
                .Include(r => r.Prostor)
                .Where(r => r.KorisnikId == korisnikId)
                .ToListAsync();

            var sada = DateTime.Now;

            var rezervacije = rezervacijeEntities.Select(r =>
            {
                var status = r.Status;

                if (status != "Otkazano")
                {
                    status = (r.VrijemeDo < sada) ? "Prošlo" : "Aktivno";
                }

                return new Rezervacija(
                    r.Id,
                    new Space(
                        r.Prostor.Id,
                        r.Prostor.Naziv,
                        r.Prostor.Kapacitet,
                        r.Prostor.Opremljenost,
                        r.Prostor.Opis,
                        (Dom)r.Prostor.DomId,
                        (TipProstora)r.Prostor.TipProstorijeId,
                        r.Prostor.SlikaPutanja
                    ),
                    r.KorisnikId,
                    r.VrijemeOd,
                    r.VrijemeDo,
                    status,
                    r.BrojOsoba,
                    r.DatumKreiranja
                );
            }).ToList();

            return rezervacije;
        }

        public async Task OtkaziRezervaciju(int rezervacijaId)
        {
            await using var db = new Campus4UContext();
            var rezervacija = await db.Rezervacije
                .FirstOrDefaultAsync(r => r.Id == rezervacijaId);
            if (rezervacija != null)
            {
                rezervacija.Status = "Otkazano";
                await db.SaveChangesAsync();
            }
        }

        public async Task UrediRezervaciju(Rezervacija rezervacija)
        {
            await using var db = new Campus4UContext();
            var postojećaRezervacija = await db.Rezervacije
                .FirstOrDefaultAsync(r => r.Id == rezervacija.ID);
            if (postojećaRezervacija != null)
            {
                postojećaRezervacija.VrijemeOd = rezervacija.PocetnoVrijeme;
                postojećaRezervacija.VrijemeDo = rezervacija.KrajnjeVrijeme;
                postojećaRezervacija.BrojOsoba = rezervacija.BrojOsoba;
                await db.SaveChangesAsync();
            }
        }

        public async Task<bool> KorisnikImaPreklapanje(int korisnikId, DateTime pocetnoVrijeme, DateTime krajnjeVrijeme, int? izuzmiRezervacijaId = null)
        {
            await using var db = new Campus4UContext();
            var query = db.Rezervacije
                .Where(r => r.KorisnikId == korisnikId
                            && r.Status != "Otkazano"
                            && pocetnoVrijeme < r.VrijemeDo
                            && krajnjeVrijeme > r.VrijemeOd);
            if (izuzmiRezervacijaId.HasValue)
            {
                query = query.Where(r => r.Id != izuzmiRezervacijaId.Value);
            }
            var postojiPreklapanje = await query.AnyAsync();
            return postojiPreklapanje;
        }
    }
}
