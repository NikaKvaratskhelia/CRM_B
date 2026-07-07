using CRM_B.Domain.Kernel.Guards;
using CRM_B.Domain.Kernel.Models;
using CRM_B.Domain.Kernel.Results;
using CRM_B.Domain.Kernel.Results.Errors;
using CRM_B.Domain.Kernel.Results.Extensions;

namespace CRM_B.Domain.Aggregates.Users.ValueObjects;

public sealed class PhoneNumber : ValueObject
{
    private const string Field = "Phone number";
    private const string Pattern = @"^\+?[0-9\s\-()]{5,20}$";

    private PhoneNumber(string value) => Value = value;

    public string Value { get; }

    public static Result<PhoneNumber> Create(string? value)
    {
        var notEmpty = Guard.AgainstNullOrEmpty(value, ErrorResults.Required(Field));
        if (notEmpty.IsFailure) return notEmpty.ToFailure<PhoneNumber>();

        var trimmed = value!.Trim();

        return Guard.AgainstRegex(trimmed, Pattern, ErrorResults.InvalidFormat(Field))
            .Bind(() => Result<PhoneNumber>.Success(new PhoneNumber(trimmed)));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}