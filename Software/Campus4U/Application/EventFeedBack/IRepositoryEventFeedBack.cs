using Client.Application.EventFeedBack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// Nikola Kihas
namespace Client.Data.EventFeedBack
{
    public interface IRepositoryEventFeedBack
    {
        Task<IEnumerable<EventFeedbackComment>> DohatiSve(int idDogadaja, int idKorisnika);

        Task<IEnumerable<EventFeedbackComment>> DohatiMoje(int idDogadaja, int idKorisnika);

        bool Unesi(EventFeedbackComment komentar);
        bool Azuriraj(EventFeedbackComment komentar);
        bool Obrisi(int komentarId);
    }
}
