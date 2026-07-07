using System.Globalization;
using CRM_B.Application.Abstractions.Localization;
using CRM_B.Application.Common.Localization;
using CRM_B.Domain.Aggregates.Users.Enums;
using CRM_B.Domain.Kernel.Results.Errors;
using Microsoft.Extensions.Localization;

namespace CRM_B.Infrastructure.Localization.Localizers;

public sealed class EmailLocalizer : IEmailLocalizer
{
    private readonly IStringLocalizer _localizer;

    public EmailLocalizer(IStringLocalizerFactory factory)
    {
        _localizer = factory.Create("Emails.Emails", typeof(ErrorResults).Assembly.GetName().Name!);
    }

    public string Subject(string code, Language language, params object?[] args)
    {
        var previous = CultureInfo.CurrentUICulture;

        try
        {
            CultureInfo.CurrentUICulture = new CultureInfo(CultureResolver.ToCultureName(language));

            var localized = _localizer[code];

            return args.Length > 0 ? string.Format(localized.Value, args) : localized.Value;
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }
}