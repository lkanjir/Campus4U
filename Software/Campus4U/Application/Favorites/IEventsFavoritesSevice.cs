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
    public interface IEventsFavoritesSevice
    {
        Task<bool> DodajFavoritaDogadajaAsync(int korisnikId, int dogadajId);
        Task<bool> UkloniFavoritaDogadajaAsync(int korisnikId, int dogadajId);
        Task<List<Event>> DohvatiFavoriteKorisnikaAsync(int korisnikId);
        Task<List<UserProfile>> DohvatiKorisnikeZaDogadajAsync(int dogadajId);
        Task<bool> ToggleFavoritaDogadaj(int korisnikId, int dogadajId);
    }
}
