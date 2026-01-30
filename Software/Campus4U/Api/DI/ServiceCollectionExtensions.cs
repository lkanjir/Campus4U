using Api.Configuration;
using Api.Middleware;
using Api.Workers;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Application.Email;
using Server.Data;
using Server.Data.Context;
using Server.Data.Email;

namespace Api.DI;

//Luka Kanjir
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
        services.AddSingleton<ITriggerControl, TriggerControl>();
        services.AddHostedService<OutboxWorker>();

        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ConnectionStrings>().Bind(configuration.GetSection("ConnectionStrings"))
            .Validate(s => !string.IsNullOrWhiteSpace(s.Campus4U),
                "DB: Nedostaje connection string (ConnectionStrings__Campus4U) u compose.yaml")
            .ValidateOnStart();

        services.AddDbContext<Campus4UContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Campus4U")));

        return services;
    }

    public static IServiceCollection AddAuth0(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<Auth0Options>().Bind(configuration.GetSection("Auth0"))
            .Validate(o => !string.IsNullOrWhiteSpace(o.Domain), "AUTH: Nedostaje Domain u appsettings.json")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Audience), "AUTH: Nedostaije Audience u appsettings.json")
            .ValidateOnStart();

        var auth0 = configuration.GetSection("Auth0").Get<Auth0Options>()!;
        var audience = auth0.Audience!;
        var domain = auth0.Domain!;
        if (!domain.EndsWith("/")) domain += "/";

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.Authority = domain;
            options.Audience = auth0.Audience!;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = domain,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
        });

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        });

        return services;
    }
}