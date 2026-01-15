
// Tin Posavec

using Client.Domain.Fault;

namespace Client.Application.Fault
{
    public interface IFaultRepository
    {
        Task<List<FaultType>> DohvatiVrsteKvarova();
        Task<bool> SpremiKvar(FaultReport kvar);
    }
}
