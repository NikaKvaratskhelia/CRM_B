using CRM_B.Application.Abstractions.Email;
using CRM_B.Infrastructure.Common;
using CRM_B.Infrastructure.Email.Sender;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CRM_B.Infrastructure.Email;

public static class EmailExtensions
{
    public static IServiceCollection AddEmail(this IServiceCollection services, IConfiguration config)
    {
        services.AddValidatedOptions<MailKitOptions>(config, MailKitOptions.SectionName);

        var mailOptions = config.GetSection(MailKitOptions.SectionName).Get<MailKitOptions>()!;

        services.AddFluentEmail(mailOptions.FromEmail, mailOptions.FromName)
            .AddRazorRenderer();

        services.AddScoped<IEmailSender, EmailSender>();

        return services;
    }
}