using Api.Middleware;
using FluentValidation;
using Microsoft.AspNetCore.Components.Forms;
using Server.Application;
using Server.Application.Email;
using Server.Data.Email;

namespace Api.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEmail(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<SmtpSettings>().Bind(configuration.GetSection("Smtp"))
            .Validate(s => !string.IsNullOrWhiteSpace(s.Host),
                "SMTP: Host mora biti definiran (Host) u appsettings.json ili u compose.yaml (Smtp__Host)")
            .Validate(s => s.Port is > 0 and <= 65535,
                "SMTP: Port mora biti definiran (Port) u appsettings.json ili u compose.yaml (Smtp__Port)")
            .Validate(s => !string.IsNullOrWhiteSpace(s.Username),
                "SMTP: Username mora biti definiran (Username) u appsettings.json ili u compose.yaml (Smtp__Username)")
            .Validate(s => !string.IsNullOrWhiteSpace(s.Password),
                "SMTP: Password mora biti definiran (Password) u appsettings.json ili u compose.yaml (Smtp__Password)")
            .Validate(s => !string.IsNullOrWhiteSpace(s.FromEmail),
                "SMTP: FromEmail mora biti definiran (FromEmail) u appsettings.json ili u compose.yaml (Smtp__FromEmail)")
            .Validate(s => !string.IsNullOrWhiteSpace(s.FromName),
                "SMTP: FromName mora biti definiran (FromName) u appsettings.json ili u compose.yaml (Smtp__FromName)")
            .ValidateOnStart();
        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<ValidationMiddleware>(ServiceLifetime.Singleton);
        services.AddScoped<IEmailSender, EmailSender>();

        return services;
    }
}