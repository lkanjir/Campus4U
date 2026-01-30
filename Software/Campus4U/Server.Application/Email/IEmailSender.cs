namespace Server.Application.Email;

//Luka Kanjir
public interface IEmailSender
{
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}