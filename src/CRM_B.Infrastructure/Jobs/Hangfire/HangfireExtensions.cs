using CRM_B.Infrastructure.Common;
using CRM_B.Infrastructure.Jobs.Hangfire.Options;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CRM_B.Infrastructure.Jobs.Hangfire;

public static class HangfireExtensions
{
    public static IServiceCollection AddHangfire(this IServiceCollection services, IConfiguration config)
    {
        services.AddValidatedOptions<HangfireOptions>(config, HangfireOptions.SectionName);
        services.AddSingleton(sp => sp.GetRequiredService<HangfireOptions>().Dashboard);

        var connection = config.GetConnectionString("Default");

        services.AddHangfire(cfg => cfg
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseFilter(new AutomaticRetryAttribute { Attempts = 4 })
            .UsePostgreSqlStorage(o => o.UseNpgsqlConnection(connection)));

        services.AddHangfireServer((sp, o) =>
        {
            var options = sp.GetRequiredService<IOptions<HangfireOptions>>().Value;
            o.WorkerCount = options.WorkerCount;
        });

        return services;
    }
}