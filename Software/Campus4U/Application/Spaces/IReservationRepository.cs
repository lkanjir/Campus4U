using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Domain.Spaces;

namespace Client.Application.Spaces
{
    public interface IReservationRepository
    {
        Task<bool> ProvjeriDostupnost(int prostorId, DateTime pocetnoVrijeme, DateTime krajnjeVrijeme);
        Task SpremiRezervaciju(Rezervacija rezervacija);
    }
}
