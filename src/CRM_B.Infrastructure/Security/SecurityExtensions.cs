using CRM_B.Application.Abstractions.Security;
using CRM_B.Infrastructure.Common;
using CRM_B.Infrastructure.Security.Google;
using CRM_B.Infrastructure.Security.Jwt;
using CRM_B.Infrastructure.Security.Password;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VS_ARC.Infrastructure.Security.Password;

namespace CRM_B.Infrastructure.Security;

public static class SecurityExtensions
{
    public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration config)
    {
        services.AddValidatedOptions<JwtOptions>(config, JwtOptions.SectionName);
        services.AddValidatedOptions<GoogleOptions>(config, GoogleOptions.SectionName);
        services.AddValidatedOptions<PasswordHashingOptions>(config, PasswordHashingOptions.SectionName);

        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<IGoogleAuthService, GoogleService>();
        services.AddScoped<IJwtService, JwtService>();

        services.AddMemoryCache();

        return services;
    }
}