namespace Api.Configuration;

//Luka Kanjir
public static class ApiEndpoints
{
    private const string apiBase = "/api";

    public static class Notifications
    {
        private const string Base = $"{apiBase}/notifications";
        public const string Test = $"{Base}/test";
    }

    public static class Triggers
    {
        private const string Base = $"{apiBase}/triggers";
        public const string Start = $"{Base}/start";
        public const string Heartbeat = $"{Base}/heartbeat";
        public const string Kick = $"{Base}/kick";
        public const string Status = $"{Base}/status";
    }
}