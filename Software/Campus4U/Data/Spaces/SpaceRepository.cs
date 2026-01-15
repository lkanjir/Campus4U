using Client.Application.Spaces;
using Client.Data.Context;
using Client.Domain.Spaces;
using Microsoft.EntityFrameworkCore;


// Tin Posavec

namespace Client.Data.Spaces
{
    public sealed class SpaceRepository : ISpaceRepository
    {
        public async Task<List<Space>> DohvatiSveProstore()
        {
            await using var db = new Campus4UContext();

            return await db.Prostori
                .Select(p => new Space(p.Id, p.Naziv))
                .ToListAsync();
        }
    }
}
