
// Tin Posavec

using Client.Domain.Fault;

namespace Client.Application.Fault
{
    public interface IFaultRepository
    {
        Task<List<FaultType>> DohvatiVrsteKvarova();
        Task<bool> SpremiKvar(FaultReport kvar);

        // FZ-11
        Task<List<FaultReport>> DohvatiSveKvarove();
        Task<FaultReport?> DohvatiKvarPoId(int kvarId);
        Task<List<FaultReport>> DohvatiKvaroveFiltrirano(string? status, int? prostorId, int? vrstaKvaraId, DateTime? odDatuma, DateTime? doDatuma);
        Task<bool> AzurirajStatusKvara(int kvarId, string noviStatus);
        Task<FaultStatistics> DohvatiStatistiku();
    }
}
