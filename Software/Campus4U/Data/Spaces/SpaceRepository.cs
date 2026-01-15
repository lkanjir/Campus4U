using Client.Application.Spaces;
using Client.Data.Context;
using Client.Domain.Spaces;


// Tin Posavec

namespace Client.Data.Spaces
{
    public sealed class SpaceRepository : ISpaceRepository
    {
        public List<Space> DohvatiSveProstore()
        {
            using var db = new Campus4UContext();

            var prostori = db.Prostori
                .Select(p => new Space (p.Id, p.Naziv))
                .ToList();

            return prostori;    
        }
    }
}
