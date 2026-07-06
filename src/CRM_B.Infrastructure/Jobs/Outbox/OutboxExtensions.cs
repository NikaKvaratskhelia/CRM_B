using CRM_B.Infrastructure.Common;
using CRM_B.Infrastructure.Jobs.Outbox.Cleanup;
using CRM_B.Infrastructure.Jobs.Outbox.Options;
using CRM_B.Infrastructure.Jobs.Outbox.Processing;
using CRM_B.Infrastructure.Jobs.Outbox.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CRM_B.Infrastructure.Jobs.Outbox;

public static class OutboxExtensions
{
    public static IServiceCollection AddOutbox(this IServiceCollection services, IConfiguration config)
    {
        services.AddValidatedOptions<OutboxOptions>(config, OutboxOptions.SectionName);

        services.AddSingleton<DomainEventTypeRegistry>();
        services.AddScoped<OutboxProcessor>();
        services.AddScoped<OutboxCleanup>();

        return services;
    }
}