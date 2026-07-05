using CRM_B.Infrastructure.Email.Sender;
using CRM_B.Infrastructure.Persistence.Data;
using Hangfire;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace CRM_B.Api.Hosting.Health;

public static class HealthCheckExtensions
{
    private const string ReadyTag = "ready";

    public static IServiceCollection AddAppHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<DataContext>(name: "database", tags: [ReadyTag])
            .AddCheck<HangfireHealthCheck>(name: "hangfire", tags: [ReadyTag])
            .AddCheck<SendGridHealthCheck>(name: "email", tags: [ReadyTag]);

        return services;
    }

    public static WebApplication MapAppHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health/live", new() { Predicate = _ => false });
        app.MapHealthChecks("/health/ready", new() { Predicate = r => r.Tags.Contains(ReadyTag) });

        return app;
    }
}

internal sealed class HangfireHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var stats = JobStorage.Current.GetMonitoringApi().GetStatistics();
            return Task.FromResult(HealthCheckResult.Healthy(
                $"servers={stats.Servers}, enqueued={stats.Enqueued}, failed={stats.Failed}"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Hangfire storage unreachable", ex));
        }
    }
}

internal sealed class SendGridHealthCheck(IOptions<SendGridOptions> options) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(string.IsNullOrWhiteSpace(options.Value.Key)
            ? HealthCheckResult.Unhealthy("SendGrid API key is not configured")
            : HealthCheckResult.Healthy());
}