using CRM_B.Domain.Kernel.Guards;
using CRM_B.Domain.Kernel.Models;
using CRM_B.Domain.Kernel.Results;
using CRM_B.Domain.Kernel.Results.Errors;
using CRM_B.Domain.Kernel.Results.Extensions;

namespace CRM_B.Domain.Aggregates.Users.ValueObjects;

public sealed class Email : ValueObject
{
    private const string Field = "Email";
    private const int MaxLength = 254;
    private const string Pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

    private Email()
    {
    }

    private Email(string value) => Value = value;

    public string Value { get; }

    public static Result<Email> Create(string? value)
    {
        var notEmpty = Guard.AgainstNullOrEmpty(value, ErrorResults.Required(Field));
        if (notEmpty.IsFailure) return notEmpty.ToFailure<Email>();

        var normalized = value!.Trim().ToLowerInvariant();

        return Guard.AgainstStringRange(normalized, 1, MaxLength, ErrorResults.InvalidLength(Field, 1, MaxLength))
            .Bind(() => Guard.AgainstRegex(normalized, Pattern, ErrorResults.InvalidFormat(Field)))
            .Bind(() => Result<Email>.Success(new Email(normalized)));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}