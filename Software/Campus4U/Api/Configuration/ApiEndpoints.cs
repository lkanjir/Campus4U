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

    public static class Images
    {
        private const string Base = $"{apiBase}/images";
        public const string UploadEvent = $"{Base}/events";
        public const string GetEvent =  $"{Base}/events/{{eventId:int}}";
        public const string UploadFault = $"{Base}/faults";
        public const string GetFault =  $"{Base}/faults/{{faultId:int}}";
    }
}