using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Domain.Spaces;

//Marko Mišić

namespace Client.Application.Spaces
{
    public interface IReservationRepository
    {
        Task<bool> ProvjeriDostupnost(int prostorId, DateTime pocetnoVrijeme, DateTime krajnjeVrijeme);
        Task SpremiRezervaciju(Rezervacija rezervacija);
        Task<int> DohvatiZauzetoMjesta(int prostorId, DateTime pocetnoVrijeme, DateTime krajnjeVrijeme);
        Task<List<Rezervacija>> DohvatiRezervacijeKorisnika(int korisnikId);
    }
}
