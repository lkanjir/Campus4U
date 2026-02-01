using Client.Domain.Notifications;

namespace Client.Application.Notifications;

//Luka Kanjir
public sealed record SaveNotificationPreferencesResult(bool IsSuccess, string? Error);

//Luka Kanjir
public sealed class NotificationPreferenceService(INotificationPreferencesRepository repository)
{
    public Task<NotificationPreferences> GetByUserIdAsync(int userId, CancellationToken ct = default) =>
        userId <= 0 ? Task.FromResult(NotificationPreferences.Default) : repository.GetByUserIdAsync(userId, ct);

    public async Task<SaveNotificationPreferencesResult> SaveAsync(int userId, NotificationPreferences preferences,
        CancellationToken ct = default)
    {
        if (userId <= 0)
            return new SaveNotificationPreferencesResult(false, "Neispravan korisnik.");

        await repository.SaveAsync(userId, preferences, ct);
        return new SaveNotificationPreferencesResult(true, null);
    }
}