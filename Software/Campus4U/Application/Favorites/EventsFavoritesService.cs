using Client.Domain.Events;
using Client.Domain.Spaces;
using Client.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// Nikola Kihas
namespace Client.Application.Favorites
{
    public class EventsFavoritesService : IEventsFavoritesSevice
    {
        private readonly IEventsFavoritesRepository _repo;

        public EventsFavoritesService(IEventsFavoritesRepository repo)
        {
            _repo = repo;
        }
        private static void ValidirajId(int id, string name)
        {
            if (id <= 0)
            {
                throw new ArgumentException($"{name} mora biti > 0.", name);
            }
        }

        public Task<bool> DodajFavoritaDogadajaAsync(int korisnikId, int dogadajId)
        {
            ValidirajId(korisnikId, nameof(korisnikId));
            ValidirajId(dogadajId, nameof(dogadajId));
            return _repo.DodajFavoritaDogadajaAsync(korisnikId, dogadajId);
        }
        public Task<bool> UkloniFavoritaDogadajaAsync(int korisnikId, int dogadajId)
        {
            ValidirajId(korisnikId, nameof(korisnikId));
            ValidirajId(dogadajId, nameof(dogadajId));
            return _repo.UkloniFavoritaDogadajaAsync(korisnikId, dogadajId);
        }

        public Task<List<Event>> DohvatiFavoriteKorisnikaAsync(int korisnikId)
        {
            ValidirajId(korisnikId, nameof(korisnikId));
            return _repo.DohvatiFavoriteKorisnikaAsync(korisnikId);
        }

        public Task<List<UserProfile>> DohvatiKorisnikeZaDogadajAsync(int dogadajId)
        {
            ValidirajId(dogadajId, nameof(dogadajId));
            return _repo.DohvatiKorisnikeZaDogadajAsync(dogadajId);
        }

        public Task<bool> ToggleFavoritaDogadaj(int korisnikId, int dogadajId)
        {
            ValidirajId(korisnikId, nameof(korisnikId));
            ValidirajId(dogadajId, nameof(dogadajId));
            return _repo.ToggleFavoritaDogadaja(korisnikId, dogadajId);
        }

        
    }
}
