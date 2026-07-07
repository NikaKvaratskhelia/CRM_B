using System.Reflection;
using CRM_B.Domain.Kernel.Results.Errors;
using Microsoft.Extensions.Localization;

namespace CRM_B.Infrastructure.Localization;

internal static class ErrorResourceSelfCheck
{
    public static void Verify(IStringLocalizerFactory factory)
    {
        var localizer = factory.Create("Errors.Errors", typeof(ErrorResults).Assembly.GetName().Name!);
        var codes = DiscoverErrorCodes().ToList();
        var missing = codes.Where(c => localizer[c].ResourceNotFound).ToList();

        if (missing.Count > 0)
            throw new InvalidOperationException(
                "Missing localized resource entries for error code(s): " + string.Join(", ", missing));
    }

    private static IEnumerable<string> DiscoverErrorCodes()
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);

        foreach (var type in typeof(ErrorResults).Assembly.GetTypes())
        {
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (field.FieldType != typeof(ErrorResults)) continue;
                if (field.GetValue(null) is not ErrorResults error) continue;
                if (seen.Add(error.Code)) yield return error.Code;
            }
        }
    }
}