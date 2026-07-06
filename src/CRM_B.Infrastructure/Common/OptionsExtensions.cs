using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CRM_B.Infrastructure.Common;

internal static class OptionsExtensions
{
    public static IServiceCollection AddValidatedOptions<TOptions>(
        this IServiceCollection services,
        IConfiguration config,
        string sectionName
    ) where TOptions : class
    {
        services.AddOptions<TOptions>()
            .Bind(config.GetSection(sectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton(sp => sp.GetRequiredService<IOptions<TOptions>>().Value);

        return services;
    }
}