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
                Status = rezervacija.Status
            };

            db.Rezervacije.Add(novaRezervacija);
            await db.SaveChangesAsync();
        }
    }
}
