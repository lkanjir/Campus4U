using Client.Domain.Spaces;
using Client.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Application.Favorites
{
    public interface IFavoritesService
    {
        Task<bool> DodajFavoritaProstorijeAsync(int korisnikId, int prostorijaId);
        Task<bool> UkloniFavoritaProstorijeAsync(int korisnikId, int prostorijaId);
        Task<List<Space>> DohvatiFavoriteKorisnikaAsync(int korisnikId);
        Task<List<UserProfile>> DohvatiKorisnikeZaProstorijuAsync(int prostorijaId);
        Task<bool> ToggleFavorita(int korisnikId, int prostorijaId);
    }
}
