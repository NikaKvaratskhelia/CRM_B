using CRM_B.Infrastructure.Email.Sender;
using CRM_B.Infrastructure.Persistence.Data;
using Hangfire;
using MailKit.Net.Smtp;
using MailKit.Security;
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
            .AddCheck<MailKitHealthCheck>(name: "email", tags: [ReadyTag]); // Updated name here

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

internal sealed class MailKitHealthCheck(IOptions<MailKitOptions> options) : IHealthCheck
{
    private readonly MailKitOptions _options = options.Value;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.Host) || _options.Port == 0)
        {
            return HealthCheckResult.Unhealthy("MailKit SMTP Host or Port is not configured.");
        }

        try
        {
            using var client = new SmtpClient();
            var secureSocket = _options.Port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;

            client.Timeout = 5000;
            await client.ConnectAsync(_options.Host, _options.Port, secureSocket, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            return HealthCheckResult.Healthy("SMTP server is reachable.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"Failed to connect to SMTP server at {_options.Host}:{_options.Port}",
                ex);
        }
    }
}