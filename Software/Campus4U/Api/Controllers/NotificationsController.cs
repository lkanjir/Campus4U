using Api.Mappers;
using Contracts.Notifications;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Email;

namespace Api.Controllers;

//Luka Kanjir
[ApiController]
public sealed class NotificationsController(IEmailSender emailSender, IValidator<SendEmailRequest> validator)
    : ControllerBase
{
    [HttpPost(ApiEndpoints.Notifications.Test)]
    public async Task<IActionResult> SendEmail([FromBody] SendEmailRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        await emailSender.SendAsync(request.MapToEmail(), cancellationToken);
        return NoContent();
    }
}