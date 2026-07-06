using CRM_B.Domain.Kernel.Guards;
using CRM_B.Domain.Kernel.Results;
using CRM_B.Domain.Kernel.Results.Errors;
using CRM_B.Domain.Kernel.Results.Extensions;

namespace CRM_B.Domain.ValueObjects;

public record Email
{
    private const string Field = "Email";
    private const int MaxLength = 254;
    private const string Pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.Contains("@"))
        {
            throw new ArgumentException("არასწორი იმეილის ფორმატი");
        }

        Value = value;
    }

    public string Value { get; init; }

    public static Result<Email> Create(string? value)
    {
        var notEmpty = Guard.AgainstNullOrEmpty(value, ErrorResults.Required(Field));
        if (notEmpty.IsFailure) return notEmpty.ToFailure<Email>();

        var normalized = value!.Trim().ToLowerInvariant();

        return Guard.AgainstStringRange(normalized, 1, MaxLength, ErrorResults.InvalidLength(Field, 1, MaxLength))
            .Bind(() => Guard.AgainstRegex(normalized, Pattern, ErrorResults.InvalidFormat(Field)))
            .Bind(() => Result<Email>.Success(new Email(normalized)));
    }
}