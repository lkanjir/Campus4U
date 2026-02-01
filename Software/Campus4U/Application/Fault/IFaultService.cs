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

        //Tin Posavec
        Task<List<FaultReport>> DohvatiSveKvarove();
        Task<FaultReport?> DohvatiKvarPoId(int kvarId);
        Task<List<FaultReport>> DohvatiKvaroveFiltrirano(string? status, int? prostorId, int? vrstaKvaraId, DateTime? odDatuma, DateTime? doDatuma);
        Task<bool> PromijeniStatusKvara(int kvarId, string noviStatus);
        List<string> DohvatiSveStatuse();
        List<string> DohvatiDozvoljeneStatuse(string trenutniStatus);
        Task<FaultStatistics> DohvatiStatistiku();
    }
}
