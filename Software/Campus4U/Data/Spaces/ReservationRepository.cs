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

        public async Task<List<Rezervacija>> DohvatiRezervacijeKorisnika(int korisnikId)
        {
            await using var db = new Campus4UContext();
            var rezervacijeEntities = await db.Rezervacije
                .Include(r => r.Prostor)
                .Where(r => r.KorisnikId == korisnikId)
                .ToListAsync();

            var rezervacije = rezervacijeEntities.Select(r => new Rezervacija(
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
                r.Status,
                r.BrojOsoba,
                r.DatumKreiranja.Day
            )).ToList();

            return rezervacije;
        }
    }
}
