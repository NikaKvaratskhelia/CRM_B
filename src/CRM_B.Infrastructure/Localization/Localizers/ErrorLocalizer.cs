using CRM_B.Application.Abstractions.Localization;
using CRM_B.Domain.Kernel.Results.Errors;
using Microsoft.Extensions.Localization;

namespace CRM_B.Infrastructure.Localization.Localizers;

public sealed class ErrorLocalizer : IErrorLocalizer
{
    private readonly IStringLocalizer _localizer;

    public ErrorLocalizer(IStringLocalizerFactory factory)
    {
        _localizer = factory.Create("Errors.Errors", typeof(ErrorResults).Assembly.GetName().Name!);
    }

    public string this[string code] => _localizer[code];

    public string Get(string code, params object?[] args)
    {
        var localized = _localizer[code];

        if (args.Length == 0) return localized.Value;

        return string.Format(localized.Value, args);
    }
}