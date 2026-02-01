using Client.Domain.Notifications;

namespace Client.Application.Notifications;

//Luka Kanjir
public interface INotificationPreferencesRepository
{
    Task<NotificationPreferences> GetByUserIdAsync(int userId, CancellationToken ct = default);
    Task SaveAsync(int userId, NotificationPreferences preferences, CancellationToken ct = default);
}