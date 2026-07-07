using CRM_B.Application.Options;
using CRM_B.Infrastructure.Email;
using CRM_B.Infrastructure.Jobs;
using CRM_B.Infrastructure.Localization;
using CRM_B.Infrastructure.Persistence;
using CRM_B.Infrastructure.Persistence.Idempotency;
using CRM_B.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CRM_B.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<ClientOptions>()
            .Bind(config.GetSection(ClientOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services.AddJobs(config).AddPersistence(config).AddSecurity(config)
            .AddEmail(config).AddAppLocalization().AddIdempotency(config);
    }
}