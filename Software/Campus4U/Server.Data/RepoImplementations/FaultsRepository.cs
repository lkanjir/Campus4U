using Microsoft.EntityFrameworkCore;
using Server.Application.Repositories;
using Server.Data.Context;

namespace Server.Data.RepoImplementations;

//Luka Kanjir
public sealed class FaultsRepository(Campus4UContext db) : IFaultsRepository
{
    public async Task<FaultImageInfo?> GetImageInfoAsync(int faultId, CancellationToken ct = default)
    {
        return await (from k in db.Kvarovi.AsNoTracking()
            where k.KvarId == faultId
            select new FaultImageInfo(k.KvarId, k.KorisnikId, k.SlikaPutanja)).FirstOrDefaultAsync(ct);
    }

    public async Task<bool> SetImagePathAsync(int faultId, string imagePath, CancellationToken ct = default)
    {
        var kvar = await db.Kvarovi.FirstOrDefaultAsync(k => k.KvarId == faultId, ct);
        if (kvar is null) return false;

        kvar.SlikaPutanja = imagePath;
        await db.SaveChangesAsync(ct);
        return true;
    }
}