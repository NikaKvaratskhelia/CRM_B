using CRM_B.Application.Abstractions.Persistence;
using CRM_B.Application.Abstractions.Security;
using CRM_B.Infrastructure.Persistence.Data;
using CRM_B.Infrastructure.Security.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CRM_B.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(options =>
            configuration.GetSection("JwtSettings").Bind(options));

        services.AddSingleton<IJwtService, JwtService>();

        services.AddDbContext<DataContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        services.AddScoped<IDataContext>(sp => sp.GetRequiredService<DataContext>());
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}