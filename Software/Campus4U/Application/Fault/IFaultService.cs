using Client.Domain.Fault;
using Client.Domain.Spaces;

//Tin Posavec

namespace Client.Application.Fault
{
    public interface IFaultService
    {
        Task<List<Space>> DohvatiProstore();
        Task<List<FaultType>> DohvatiVrsteKvarova();
        Task<bool> PrijaviKvar(int prostorId, int vrstaKvaraId, string opis, byte[]? fotografija, int korisnikId);
    }
}
