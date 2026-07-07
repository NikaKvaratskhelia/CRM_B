using CRM_B.Application.Abstractions.Persistence;
using CRM_B.Infrastructure.Persistence.Data;
using CRM_B.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CRM_B.Infrastructure.Persistence;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<OutboxInterceptor>();
        services.AddScoped<AuditingInterceptor>();

        services.AddDbContext<DataContext>((sp, op) =>
        {
            op.AddInterceptors(
                sp.GetRequiredService<AuditingInterceptor>(),
                sp.GetRequiredService<OutboxInterceptor>());
            op.UseNpgsql(config.GetConnectionString("Default"));
            op.UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IDataContext>(sp => sp.GetRequiredService<DataContext>());
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}