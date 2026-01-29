using Contracts.Notifications;
using FluentValidation;

namespace Api.Validators;

public sealed class SendEmailRequestValidator : AbstractValidator<SendEmailRequest>
{
    public SendEmailRequestValidator()
    {
        RuleFor(x => x.ToEmail).NotEmpty().WithMessage("Primatelj (toEmail) je obavezan.").EmailAddress()
            .WithMessage("Primatelj (toEmail) mora biti valjana email adresa");

        RuleFor(x => x.Subject).NotEmpty().WithMessage("Subject ne smije biti prazan.").MaximumLength(200)
            .WithMessage("Subject ne smije biti dulji od 200 znakova");

        RuleFor(x => x.Body).NotEmpty().WithMessage("Body ne smije biti prazan.").MaximumLength(10_000)
            .WithMessage("Body ne smije biti dlji od 10 000 znakova.");

        RuleFor(x => x.ToName).MaximumLength(200)
            .WithMessage("Ime primatelja (toName) ne smije bitti dulje od 200 znakova.")
            .When(x => x.ToName is not null);
    }
}