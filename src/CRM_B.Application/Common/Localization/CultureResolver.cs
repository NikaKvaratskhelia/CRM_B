using System.Globalization;
using CRM_B.Domain.Aggregates.Users.Enums;

namespace CRM_B.Application.Common.Localization;

public static class CultureResolver
{
    public static Language Current() =>
        CultureInfo.CurrentUICulture.TwoLetterISOLanguageName
            .Equals("ka", StringComparison.OrdinalIgnoreCase)
            ? Language.Ka
            : Language.En;

    public static string ToCultureName(Language language) => language switch
    {
        Language.Ka => "ka",
        _ => "en",
    };
}