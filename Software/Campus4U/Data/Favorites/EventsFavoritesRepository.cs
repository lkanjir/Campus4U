using Client.Application.Favorites;
using Client.Data.Context;
using Client.Data.Entities;
using Client.Domain.Events;
using Client.Domain.Spaces;
using Client.Domain.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Data.Favorites
{
    public class EventsFavoritesRepository : IEventsFavoritesRepository
    {
        public async Task<bool> DodajFavoritaDogadajaAsync(int korisnikId, int dogadajId)
        {
            await using var context = new Campus4UContext();

            var postoji = await(from df in context.DogadajiFavoriti
                                where df.KorisnikId == korisnikId && df.DogadajId == dogadajId
                                select df).AnyAsync();

            if (postoji) return false;

            context.DogadajiFavoriti.Add(new DogadajiFavoriti
            {
                KorisnikId = korisnikId,
                DogadajId = dogadajId
            });

            await context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UkloniFavoritaDogadajaAsync(int korisnikId, int dogadajId)
        {
            await using var context = new Campus4UContext();

            var dogadaj = await(from df in context.DogadajiFavoriti
                                where df.KorisnikId == korisnikId && df.DogadajId == dogadajId
                                select df).FirstOrDefaultAsync();

            if (dogadaj is null) return false;

            context.DogadajiFavoriti.Remove(dogadaj);

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Event>> DohvatiFavoriteKorisnikaAsync(int korisnikId)
        {
            await using var context = new Campus4UContext();

            return await(from df in context.DogadajiFavoriti
                         where df.KorisnikId == korisnikId
                         select new Event(
                             df.Dogadaj.Id,
                             df.Dogadaj.Naslov,
                             df.Dogadaj.Opis,
                             df.Dogadaj.VrijemeObjave,
                             df.Dogadaj.VrijemeDogadaja,
                             df.Dogadaj.Slika
                           )
                          ).AsNoTracking().ToListAsync();
        }

        public async Task<List<UserProfile>> DohvatiKorisnikeZaDogadajAsync(int dogadajId)
        {
            await using var context = new Campus4UContext();

            return await(from df in context.DogadajiFavoriti
                         where df.DogadajId == dogadajId
                         select new UserProfile(
                             df.Korisnik.Id,
                             df.Korisnik.Sub,
                             df.Korisnik.Email,
                             df.Korisnik.Ime,
                             df.Korisnik.Prezime,
                             df.Korisnik.KorisnickoIme,
                             df.Korisnik.BrojSobe,
                             df.Korisnik.BrojTelefona,
                             df.Korisnik.SlikaProfila,
                             df.Korisnik.UlogaId
                           )
                          ).AsNoTracking().ToListAsync();
        }

        public async Task<bool> ToggleFavoritaDogadaja(int korisnikId, int dogadajId)
        {
            await using var context = new Campus4UContext();

            var favorit = await(from df in context.DogadajiFavoriti
                                where df.KorisnikId == korisnikId && df.DogadajId == dogadajId
                                select df).FirstOrDefaultAsync();

            if (favorit is null)
            {
                context.DogadajiFavoriti.Add(new DogadajiFavoriti { KorisnikId = korisnikId, DogadajId = dogadajId });

                await context.SaveChangesAsync();
                return true;
            }

            context.DogadajiFavoriti.Remove(favorit);
            await context.SaveChangesAsync();
            return false;
        }
    }
}
