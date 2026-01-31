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
    public interface ISpacesFavoritesRepository
    {
        Task<bool> DodajFavoritaProstorijeAsync(int korisnikId, int prostorijaId);
        Task<bool> UkloniFavoritaProstorijeAsync(int korisnikId, int prostorijaId);
        Task<List<Space>> DohvatiFavoriteKorisnikaAsync(int korisnikId);
        Task<List<UserProfile>> DohvatiKorisnikeZaProstorijuAsync(int prostorijaId);
        Task<bool> ToggleFavoritaProstora(int korisnikId, int prostorId);
    }
}
