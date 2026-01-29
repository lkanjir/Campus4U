using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Server.Application.Email;

namespace Server.Data.Email;

//Luka Kanjir
public sealed class EmailSender(IOptions<SmtpSettings> settings) : IEmailSender
{
    private readonly SmtpSettings _settings = settings.Value;

    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(message.Subject.Trim()))
            throw new InvalidOperationException("Subject ne smije biti prazan!");
        if (string.IsNullOrWhiteSpace(message.Body.Trim()))
            throw new InvalidOperationException("Body ne smije biti prazan!");

        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
        email.To.Add(new MailboxAddress(message.ToName, message.ToEmail));
        email.Subject = message.Subject;
        var body = new BodyBuilder();
        body.TextBody = message.Body;
        email.Body = body.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.Auto, cancellationToken);
        await client.AuthenticateAsync(_settings.Username, _settings.Password, cancellationToken);
        await client.SendAsync(email, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}