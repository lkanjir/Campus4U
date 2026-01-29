using Api.Mappers;
using Contracts.Notifications;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Email;

namespace Api.Controllers;

//Luka Kanjir
[ApiController]
public class NotificationsController(IEmailSender emailSender) : ControllerBase
{
    [HttpPost(ApiEndpoints.Notifications.Test)]
    public async Task<IActionResult> SendTestEmail([FromBody] SendEmailRequest request, CancellationToken cancellationToken)
    {
        var email = request.MapToEmail();
        await emailSender.SendAsync(email,  cancellationToken);
        return NoContent();
    }
}