using CRM_B.Application.Abstractions.Idempotency;
using CRM_B.Application.Options;
using CRM_B.Infrastructure.Jobs.Idempotency;
using CRM_B.Infrastructure.Persistence.Idempotency.Store;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CRM_B.Infrastructure.Persistence.Idempotency;

public static class IdempotencyExtensions
{
    public const string PruneJobId = "idempotency-prune";

    public static IServiceCollection AddIdempotency(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<IdempotencyOptions>()
            .Bind(config.GetSection(IdempotencyOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<IIdempotencyStore, IdempotencyStore>();
        services.AddScoped<IdempotencyPruneJob>();

        return services;
    }

    public static void SchedulePrune(IRecurringJobManager jobs, IdempotencyOptions options) =>
        jobs.AddOrUpdate<IdempotencyPruneJob>(PruneJobId,
            j => j.RunAsync(CancellationToken.None), options.PruneCron);
}