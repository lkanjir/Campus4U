using Client.Application.Notifications;
using Client.Data.Context;
using Client.Data.Entities;
using Client.Domain.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Client.Data.Notifications;

public class NotificationPreferenceRepository : INotificationPreferencesRepository
{
    public async Task<NotificationPreferences> GetByUserIdAsync(int userId, CancellationToken ct = default)
    {
        await using var db = new Campus4UContext();
        var rows = await (from o in db.ObavijestiPostavke.AsNoTracking() where o.KorisnikId == userId select o)
            .ToListAsync(ct);

        if (rows.Count == 0) return NotificationPreferences.Default;
        bool Get(string type) => rows.FirstOrDefault(r => r.Tip == type)?.Omoguceno ?? true;

        return new NotificationPreferences(
            Get(NotificationPreferenceTypes.Posts),
            Get(NotificationPreferenceTypes.Faults),
            Get(NotificationPreferenceTypes.Reservations));
    }

    public async Task SaveAsync(int userId, NotificationPreferences preferences, CancellationToken ct = default)
    {
        await using var db = new Campus4UContext();
        var existing = await (from o in db.ObavijestiPostavke where o.KorisnikId == userId select o).ToListAsync(ct);
        
        Upsert(NotificationPreferenceTypes.Posts, preferences.Posts);
        Upsert(NotificationPreferenceTypes.Faults, preferences.Faults);
        Upsert(NotificationPreferenceTypes.Reservations, preferences.Reservations);
        
        await db.SaveChangesAsync(ct);
        return;

        void Upsert(string type, bool enabled)
        {
            var entity = existing.FirstOrDefault(p => p.Tip == type);
            if (entity is null)
            {
                db.ObavijestiPostavke.Add(new ObavijestiPostavke
                {
                    KorisnikId = userId,
                    Tip = type,
                    Omoguceno = enabled,
                    Azurirano = DateTime.Now
                });
                return;
            }
  
            entity.Omoguceno = enabled;
            entity.Azurirano = DateTime.Now;
        }
    }
    
    
}