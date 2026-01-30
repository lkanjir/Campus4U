namespace Api;

//Luka Kanjir
public sealed class ApiEndpoints
{
    private const string apiBase = "/api";

    public sealed class Notifications
    {
        private const string Base = $"{apiBase}/notifications";
        public const string Test = $"{Base}/test";
    }

    public sealed class Triggers
    {
        private const string Base = $"{apiBase}/triggers";
        public const string Start = $"{Base}/start";
        public const string Heartbeat = $"{Base}/heartbeat";
        public const string Kick = $"{Base}/kick";
        public const string Status = $"{Base}/status";
    }
}