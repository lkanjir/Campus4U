namespace Contracts.Notifications;

//Luka Kanjir
public sealed class SendEmailRequest
{
    public required string ToEmail { get; init; }
    public string? ToName {get; init; }
    public required string Subject { get; init; }
    public required string Body { get; init; }
}