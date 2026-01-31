using Microsoft.EntityFrameworkCore;
using Server.Application.Events;
using Server.Data.Context;

namespace Server.Data.Events;

public sealed class EventsRepository(Campus4UContext db) : IEventsRepository
{
    public async Task<EventImageInfo?> GetImageInfoAsync(int eventId, CancellationToken ct = default)
    {
        return await (from d in db.Dogadaji.AsNoTracking()
            where d.Id == eventId
            select new EventImageInfo(d.Id, d.SlikaPutanja)).FirstOrDefaultAsync(ct);
    }

    public async Task<bool> SetImagePathAsync(int eventId, string imagePath, CancellationToken ct = default)
    {
        var dogadjaj = await db.Dogadaji.FirstOrDefaultAsync(d => d.Id == eventId, ct);
        if (dogadjaj is null) return false;

        dogadjaj.SlikaPutanja = imagePath;
        await db.SaveChangesAsync(ct);
        return true;
    }
}