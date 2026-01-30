using Contracts.Notifications;
using Server.Application.Email;

namespace Api.Mappers;

public static class EmailMapper
{
    public static EmailMessage MapToEmail(this SendEmailRequest request)
    {
        return new EmailMessage
        {
            ToEmail = request.ToEmail,
            ToName = request.ToName,
            Subject = request.Subject,
            Body = request.Body,
        };
    }
    
}