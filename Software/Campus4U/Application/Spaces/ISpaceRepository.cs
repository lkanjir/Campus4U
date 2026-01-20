

//Tin Posavec, Marko Mišić

using Client.Domain.Spaces;

namespace Client.Application.Spaces
{
    public interface ISpaceRepository
    {
        Task<List<Space>> DohvatiSveProstore();
        Task<List<Space>> DohvatiFilter(string? naziv, TipProstora? tip);
    }
}
