using Client.Data.Context.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Data.EventFeedBack
{
    public interface IRepositoryEventFeedBack
    {
        Task<IEnumerable<KomentariDogadaja>> DohatiSve(int idDogadaja);

        Task<IEnumerable<KomentariDogadaja>> DohatiMoje(int idDogadaja, int idKorisnika);

        void Unesi(KomentariDogadaja komentar);
        void Azuriraj(KomentariDogadaja komentar);
        void Obrisi(KomentariDogadaja komentar);
    }
}
