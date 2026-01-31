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
    public class SpacesFavoritesService : ISpacesFavoritesService
    {
        private readonly ISpacesFavoritesRepository _repo;

        public SpacesFavoritesService(ISpacesFavoritesRepository repo)
        {
            _repo = repo;
        }
        private static void ValidirajId(int id, string name)
        {
            if(id <= 0)
            {
                throw new ArgumentException($"{name} mora biti > 0.", name);
            }
        }

        public Task<bool> DodajFavoritaProstorijeAsync(int korisnikId, int prostorijaId)
        {
            ValidirajId(korisnikId, nameof(korisnikId));
            ValidirajId(prostorijaId, nameof(prostorijaId));
            return _repo.DodajFavoritaProstorijeAsync(korisnikId, prostorijaId);
        }
        public Task<bool> UkloniFavoritaProstorijeAsync(int korisnikId, int prostorijaId)
        {
            ValidirajId(korisnikId, nameof(korisnikId));
            ValidirajId(prostorijaId, nameof(prostorijaId));
            return _repo.UkloniFavoritaProstorijeAsync(korisnikId, prostorijaId);
        }

        public Task<List<Space>> DohvatiFavoriteKorisnikaAsync(int korisnikId)
        {
            ValidirajId(korisnikId, nameof(korisnikId));
            return _repo.DohvatiFavoriteKorisnikaAsync(korisnikId);
        }

        public Task<List<UserProfile>> DohvatiKorisnikeZaProstorijuAsync(int prostorijaId)
        {
            ValidirajId(prostorijaId, nameof(prostorijaId));
            return _repo.DohvatiKorisnikeZaProstorijuAsync(prostorijaId);
        }

        public Task<bool> ToggleFavoritaProstora(int korisnikId, int prostorijaId)
        {
            ValidirajId(korisnikId, nameof(korisnikId));
            ValidirajId(prostorijaId, nameof(prostorijaId));
            return _repo.ToggleFavoritaProstora(korisnikId, prostorijaId);
        }
    }
}
