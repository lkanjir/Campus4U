namespace Api;

public sealed class ApiEndpoints
{
    private const string apiBase = "/api";

    public sealed class Notifications
    {
        private const string Base = $"{apiBase}/notifications";
        public const string Test = $"{Base}/test";
    }
}