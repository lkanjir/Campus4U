

//Tin Posavec

using Client.Domain.Spaces;

namespace Client.Application.Spaces
{
    public interface ISpaceRepository
    {
        List<Space> DohvatiSveProstore();
    }
}
