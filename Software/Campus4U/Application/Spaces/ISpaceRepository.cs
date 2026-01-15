

//Tin Posavec

using Client.Domain.Spaces;

namespace Client.Application.Spaces
{
    public interface ISpaceRepository
    {
        Task<List<Space>> DohvatiSveProstore();
    }
}
