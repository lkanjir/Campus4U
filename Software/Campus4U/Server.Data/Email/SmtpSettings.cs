namespace Server.Data.Email;

//Luka Kanjir
public sealed class SmtpSettings
{
    public required string Host { get; init; }
    public int Port { get; init; } = 587;
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string FromEmail { get; init; }
    public string? FromName { get; init; }
}