using Client.Data.EventFeedBack;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.Application.EventFeedBack
{
    public sealed class EventFeedBackService : IEventFeedBackService
    {
        private readonly IRepositoryEventFeedBack _repository;

        public EventFeedBackService(IRepositoryEventFeedBack repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Task<IEnumerable<EventFeedbackComment>> DohatiSve(int idDogadaja)
        {
            if (idDogadaja <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(idDogadaja));
            }

            return _repository.DohatiSve(idDogadaja);
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

            return _repository.DohatiMoje(idDogadaja, idKorisnika);
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

            return _repository.Unesi(komentar);
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

            return _repository.Azuriraj(komentar);
        }

        public bool Obrisi(int komentarId)
        {
            if (komentarId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(komentarId));
            }

            return _repository.Obrisi(komentarId);
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
