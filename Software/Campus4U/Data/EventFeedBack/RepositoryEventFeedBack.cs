using Client.Application.EventFeedBack;
using Client.Data.Context;
using Client.Data.Context.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Data.EventFeedBack
{
    public sealed class RepositoryEventFeedBack : IRepositoryEventFeedBack
    {
        public bool Azuriraj(EventFeedbackComment komentar)
        {
            using (var db = new Campus4UContext())
            {
                var entity = db.KomentariDogadaja.FirstOrDefault(k => k.Id == komentar.Id);
                if (entity is null)
                {
                    return false;
                }

                entity.Datum = komentar.Datum;
                entity.Ocjena = komentar.Ocjena;
                entity.Komentar = komentar.Komentar;
                entity.DogadajId = komentar.DogadajId;
                entity.KorisnikId = komentar.KorisnikId;

                db.SaveChanges();

                return true;
            }
        }

        public async Task<IEnumerable<EventFeedbackComment>> DohatiMoje(int idDogadaja, int idKorisnika)
        {
           await using(var db = new Campus4UContext())
            {
                var komentar = from k in db.KomentariDogadaja
                            join u in db.Korisnici on k.KorisnikId equals u.Id into korisnici
                            from u in korisnici.DefaultIfEmpty()
                            where k.DogadajId == idDogadaja && k.KorisnikId == idKorisnika
                            select new EventFeedbackComment(
                                k.Id,
                                k.Datum,
                                k.Ocjena,
                                k.Komentar,
                                u == null ? string.Empty : ((u.Ime ?? string.Empty) + " " + (u.Prezime ?? string.Empty)).Trim(),
                                true,
                                k.DogadajId,
                                k.KorisnikId);
                return await komentar.ToListAsync();
            }
        }

        public async Task<IEnumerable<EventFeedbackComment>> DohatiSve(int idDogadaja, int idKorisnika)
        {
            await using(var db = new Campus4UContext())
            {
                var komentari = from k in db.KomentariDogadaja
                            join u in db.Korisnici on k.KorisnikId equals u.Id into korisnici
                            from u in korisnici.DefaultIfEmpty()
                            where k.DogadajId == idDogadaja
                            select new EventFeedbackComment(
                                k.Id,
                                k.Datum,
                                k.Ocjena,
                                k.Komentar,
                                u == null ? string.Empty : ((u.Ime ?? string.Empty) + " " + (u.Prezime ?? string.Empty)).Trim(),
                                idKorisnika > 0 && k.KorisnikId == idKorisnika,
                                k.DogadajId,
                                k.KorisnikId);
                return await komentari.ToListAsync();
            }
        }

        public bool Obrisi(int komentarId)
        {
            using (var db = new Campus4UContext())
            {
                var entity = db.KomentariDogadaja.FirstOrDefault(k => k.Id == komentarId);
                if (entity is null)
                {
                    return false;
                }

                db.KomentariDogadaja.Remove(entity);
                db.SaveChanges();
                return true;
            }
        }

        public bool Unesi(EventFeedbackComment komentar)
        {
            using (var db = new Campus4UContext())
            {
                var entity = new KomentariDogadaja
                {
                    Datum = komentar.Datum,
                    Ocjena = komentar.Ocjena,
                    Komentar = komentar.Komentar,
                    DogadajId = komentar.DogadajId,
                    KorisnikId = komentar.KorisnikId
                };

                db.KomentariDogadaja.Add(entity);
                db.SaveChanges();

                return true;
            }
        }
    }
}
