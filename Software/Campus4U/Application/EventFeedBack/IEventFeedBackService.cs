using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// Nikola Kihas
namespace Client.Application.EventFeedBack
{
    public interface IEventFeedBackService
    {
        Task<IEnumerable<EventFeedbackComment>> DohatiSve(int idDogadaja, int idKorisnika);

        Task<IEnumerable<EventFeedbackComment>> DohatiMoje(int idDogadaja, int idKorisnika);

        bool Dodaj(EventFeedbackComment komentar);
        bool Uredi(EventFeedbackComment komentar);
        bool Obrisi(int komentarId);
    }
}
