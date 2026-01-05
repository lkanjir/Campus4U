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
    internal class RepositoryEventFeedBack : IRepositoryEventFeedBack
    {
        public void Azuriraj(KomentariDogadaja komentar)
        {
            using (var db = new Campus4UContext())
            {
                db.KomentariDogadaja.Update(komentar);
                db.SaveChanges();
            }
        }

        public async Task<IEnumerable<KomentariDogadaja>> DohatiMoje(int idDogadaja, int idKorisnika)
        {
           await using(var db = new Campus4UContext())
            {
                var komentar = from k in db.KomentariDogadaja
                            where k.DogadajId == idDogadaja && k.KorisnikId == idKorisnika
                            select k;
                return await komentar.ToListAsync();
            }
        }

        public async Task<IEnumerable<KomentariDogadaja>> DohatiSve(int idDogadaja)
        {
            await using(var db = new Campus4UContext())
            {
                var komentari = from k in db.KomentariDogadaja
                            where k.DogadajId == idDogadaja
                            select k;
                return await komentari.ToListAsync();
            }
        }

        public void Obrisi(KomentariDogadaja komentar)
        {
            using (var db = new Campus4UContext())
            {
                db.Attach(komentar);
                db.KomentariDogadaja.Remove(komentar);
                db.SaveChanges();
            }
        }

        public void Unesi(KomentariDogadaja komentar)
        {
            using(var db = new Campus4UContext())
            {
                db.KomentariDogadaja.Add(komentar);
                db.SaveChanges();
            }
        }
    }
}
