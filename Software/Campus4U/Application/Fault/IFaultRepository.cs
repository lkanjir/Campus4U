
// Tin Posavec

using Client.Domain.Fault;

namespace Client.Application.Fault
{
    public interface IFaultRepository
    {
        List<FaultType> DohvatiVrsteKvarova();
        bool SpremiKvar(FaultReport kvar);
    }
}
