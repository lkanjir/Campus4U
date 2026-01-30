namespace Server.Application.Email;

//Luka Kanjir
public sealed class EmailMessage
{
    public required string ToEmail { get; init; }
    public string? ToName {get; init; }
    public required string Subject { get; init; }
    public required string Body { get; init; }
}