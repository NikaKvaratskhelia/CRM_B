using System.Globalization;
using CRM_B.Domain.Aggregates.Users.Enums;
using Microsoft.AspNetCore.Localization;

namespace CRM_B.Api.Extensions.Localization;

public static class LocalizationExtensions
{
    public static WebApplication UseAppLocalization(this WebApplication app)
    {
        var supportedCultures = Enum.GetValues<Language>()
            .Select(l => new CultureInfo(l.ToString().ToLowerInvariant()))
            .ToArray();

        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(nameof(Language.En).ToLowerInvariant()),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures
        });

        return app;
    }
}