using CRM_B.Application.Abstractions.Authentication;
using CRM_B.Application.Abstractions.Persistence;
using CRM_B.Infrastructure.Authentication;
using CRM_B.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CRM_B.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(options =>
            configuration.GetSection("JwtSettings").Bind(options));

        services.AddSingleton<IJwtProvider, JwtProvider>();

        services.AddDbContext<DataContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        services.AddScoped<IDataContext>(sp => sp.GetRequiredService<DataContext>());
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}