using Client.Data.EventFeedBack;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
/// Nikola Kihas
namespace Client.Application.EventFeedBack
{
    public sealed class EventFeedBackService : IEventFeedBackService
    {
        private readonly IRepositoryEventFeedBack _repo;

        public EventFeedBackService(IRepositoryEventFeedBack repository)
        {
            _repo = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Task<IEnumerable<EventFeedbackComment>> DohatiSve(int idDogadaja, int idKorisnika)
        {
            if (idDogadaja <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(idDogadaja));
            }
            if (idKorisnika <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(idKorisnika));
            }

            return _repo.DohatiSve(idDogadaja, idKorisnika);
        }

        public Task<IEnumerable<EventFeedbackComment>> DohatiMoje(int idDogadaja, int idKorisnika)
        {
            if (idDogadaja <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(idDogadaja));
            }

            if (idKorisnika <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(idKorisnika));
            }

            return _repo.DohatiMoje(idDogadaja, idKorisnika);
        }

        public bool Dodaj(EventFeedbackComment komentar)
        {
            if (komentar is null)
            {
                throw new ArgumentNullException(nameof(komentar));
            }
            if (!provjeriUnos(komentar))
            {
                throw new ArgumentException("Neispravan unos komentara.");
            }

            return _repo.Unesi(komentar);
        }

        public bool Uredi(EventFeedbackComment komentar)
        {
            if (komentar is null)
            {
                throw new ArgumentNullException(nameof(komentar));
            }
            if (!provjeriUnos(komentar))
            {
                throw new ArgumentException("Neispravan unos komentara.");
            }

            return _repo.Azuriraj(komentar);
        }

        public bool Obrisi(int komentarId)
        {
            if (komentarId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(komentarId));
            }

            return _repo.Obrisi(komentarId);
        }

        private bool provjeriUnos(EventFeedbackComment komentar)
        {
            if(komentar.Ocjena < 1 || komentar.Ocjena > 5)
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(komentar.Komentar))
            {
                return false;
            }
            if(komentar.DogadajId <=0 || komentar.KorisnikId <=0)
            {
                return false;
            }
            return true;
        }
    }
}
