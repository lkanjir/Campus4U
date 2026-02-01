namespace Client.Domain.Notifications;

//Luka Kanjir
public sealed record NotificationPreferences(bool Posts, bool Faults, bool Reservations)
{
    public static NotificationPreferences Default => new(true, true, true);
}