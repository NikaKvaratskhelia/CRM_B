using CRM_B.Application.Abstractions.Localization;
using CRM_B.Infrastructure.Localization.Localizers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace CRM_B.Infrastructure.Localization;

public static class LocalizationExtensions
{
    public static IServiceCollection AddAppLocalization(this IServiceCollection services)
    {
        services.AddLocalization(options => options.ResourcesPath = "Locales");
        services.AddSingleton<IErrorLocalizer, ErrorLocalizer>();
        services.AddSingleton<IEmailLocalizer, EmailLocalizer>();

        return services;
    }

    public static IServiceProvider VerifyErrorResources(this IServiceProvider services)
    {
        var factory = services.GetRequiredService<IStringLocalizerFactory>();
        ErrorResourceSelfCheck.Verify(factory);
        return services;
    }
}