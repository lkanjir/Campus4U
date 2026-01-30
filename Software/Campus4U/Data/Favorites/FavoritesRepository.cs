using Client.Application.Favorites;
using Client.Data.Context;
using Client.Data.Entities;
using Client.Domain.Spaces;
using Client.Domain.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Client.Data.Favorites
{
    public class FavoritesRepository : IFavoritesRepository
    {
        public async Task<bool> DodajFavoritaProstorijeAsync(int korisnikId, int prostorijaId)
        {
            await using var context = new Campus4UContext();

            var postoji = await (from pf in context.ProstoriFavoriti
                                 where pf.KorisnikId == korisnikId && pf.ProstorId == prostorijaId
                                 select pf).AnyAsync();

            if (postoji) return false;

            context.ProstoriFavoriti.Add(new ProstoriFavoriti
            {
                KorisnikId = korisnikId,
                ProstorId = prostorijaId
            });

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UkloniFavoritaProstorijeAsync(int korisnikId, int prostorijaId)
        {
            await using var context = new Campus4UContext();

            var favorit = await (from pf in context.ProstoriFavoriti
                                 where pf.KorisnikId == korisnikId && pf.ProstorId == prostorijaId
                                 select pf).FirstOrDefaultAsync();
            if (favorit is null) return false;

            context.ProstoriFavoriti.Remove(favorit);

            await context.SaveChangesAsync();
            return true;
        }
        public async Task<List<Space>> DohvatiFavoriteKorisnikaAsync(int korisnikId)
        {
            await using var context = new Campus4UContext();

            return await (from pf in context.ProstoriFavoriti
                          where pf.KorisnikId == korisnikId
                          select new Space(
                                pf.Prostor.Id,
                                pf.Prostor.Naziv,
                                pf.Prostor.Kapacitet,
                                pf.Prostor.Opremljenost,
                                pf.Prostor.Opis,
                                (Dom)pf.Prostor.DomId,
                                (TipProstora)pf.Prostor.TipProstorijeId,
                                pf.Prostor.SlikaPutanja
                              )
                          ).AsNoTracking().ToListAsync();
        }

        public async Task<List<UserProfile>> DohvatiKorisnikeZaProstorijuAsync(int prostorijaId)
        {
            await using var context = new Campus4UContext();

            return await (from pf in context.ProstoriFavoriti
                          where pf.ProstorId == prostorijaId
                          select new UserProfile(
                              pf.Korisnik.Id,
                              pf.Korisnik.Sub,
                              pf.Korisnik.Email,
                              pf.Korisnik.Ime,
                              pf.Korisnik.Prezime,
                              pf.Korisnik.KorisnickoIme,
                              pf.Korisnik.BrojSobe,
                              pf.Korisnik.BrojTelefona,
                              pf.Korisnik.SlikaProfila,
                              pf.Korisnik.UlogaId
                            )
                          ).AsNoTracking().ToListAsync();
        }

        public async Task<bool> ToggleFavorita(int korisnikId, int prostorId)
        {
            await using var context = new Campus4UContext();

            var favorit = await (from pf in context.ProstoriFavoriti
                                 where pf.KorisnikId == korisnikId && pf.ProstorId == prostorId
                                 select pf).FirstOrDefaultAsync();

            if(favorit is null)
            {
                context.ProstoriFavoriti.Add(new ProstoriFavoriti { KorisnikId = korisnikId, ProstorId = prostorId});

                await context.SaveChangesAsync();
                return true;
            }

            context.ProstoriFavoriti.Remove(favorit);
            await context.SaveChangesAsync();
            return false;
        }
    }
}
