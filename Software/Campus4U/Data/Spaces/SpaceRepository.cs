using Client.Application.Spaces;
using Client.Data.Context;
using Client.Domain.Spaces;
using Microsoft.EntityFrameworkCore;


// Tin Posavec, Marko Mišić

namespace Client.Data.Spaces
{
    public sealed class SpaceRepository : ISpaceRepository
    {
        public async Task<List<Space>> DohvatiSveProstore()
        {
            await using var db = new Campus4UContext();

            return await db.Prostori
                .Select(p => new Space(p.Id, p.Naziv, p.Kapacitet, p.Opremljenost, p.Opis, (Dom)p.DomId, (TipProstora)p.TipProstorijeId, p.SlikaPutanja))
                .ToListAsync();
        }

        public async Task<List<Space>> DohvatiFilter(string? naziv, TipProstora? tip)
        {
            await using var db = new Campus4UContext();
            var query = db.Prostori.AsQueryable();
            if (!string.IsNullOrEmpty(naziv))
            {
                query = query.Where(p => p.Naziv.Contains(naziv));
            }
            if (tip.HasValue)
            {
                query = query.Where(p => p.TipProstorijeId == (int)tip.Value);
            }
            return await query
                .Select(p => new Space(p.Id, p.Naziv, p.Kapacitet, p.Opremljenost, p.Opis, (Dom)p.DomId, (TipProstora)p.TipProstorijeId, p.SlikaPutanja))
                .ToListAsync();
        }
    }
}
