namespace Client.Domain.Notifications;

//Luka Kanjir
public static class NotificationPreferenceTypes
{
    public const string Posts = "dogadaji";
    public const string Faults = "kvarovi";
    public const string Reservations = "rezervacije";
    public static readonly string[] All = { Posts, Faults, Reservations };
}