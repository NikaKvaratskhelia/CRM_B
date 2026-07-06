using CRM_B.Application.Options;
using CRM_B.Infrastructure.Jobs.Hangfire;
using CRM_B.Infrastructure.Jobs.Hangfire.Dashboard;
using CRM_B.Infrastructure.Jobs.Hangfire.Options;
using CRM_B.Infrastructure.Jobs.Outbox;
using CRM_B.Infrastructure.Jobs.Outbox.Options;
using CRM_B.Infrastructure.Jobs.Outbox.Processing;
using CRM_B.Infrastructure.Persistence.Idempotency;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace CRM_B.Infrastructure.Jobs;

public static class JobsExtensions
{
    public static IServiceCollection AddJobs(this IServiceCollection services, IConfiguration config)
    {
        services.AddHangfire(config);
        services.AddOutbox(config);

        return services;
    }

    public static IApplicationBuilder UseJobs(this IApplicationBuilder app)
    {
        var dashboard = app.ApplicationServices.GetRequiredService<HangfireDashboardOptions>();
        var env = app.ApplicationServices.GetRequiredService<IHostEnvironment>();

        if (dashboard.Enabled)
        {
            if (!env.IsDevelopment() && string.IsNullOrWhiteSpace(dashboard.Password))
                throw new InvalidOperationException(
                    "Hangfire dashboard password is empty in a non-development environment. Set Hangfire:Dashboard:Password.");

            app.UseHangfireDashboard(dashboard.Path, new DashboardOptions
            {
                Authorization = [new HangfireDashboardAuthFilter(dashboard)],
                DisplayStorageConnectionString = false,
            });
        }

        var jobs = app.ApplicationServices.GetRequiredService<IRecurringJobManager>();
        var options = app.ApplicationServices.GetRequiredService<OutboxOptions>();
        OutboxJobScheduler.Schedule(jobs, options);

        var idempotencyOptions = app.ApplicationServices.GetRequiredService<IOptions<IdempotencyOptions>>().Value;
        IdempotencyExtensions.SchedulePrune(jobs, idempotencyOptions);

        return app;
    }
}