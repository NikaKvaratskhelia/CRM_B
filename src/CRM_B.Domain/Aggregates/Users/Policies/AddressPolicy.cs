using CRM_B.Domain.Kernel.Guards;
using CRM_B.Domain.Kernel.Results;
using CRM_B.Domain.Kernel.Results.Errors;

namespace CRM_B.Domain.Aggregates.Users.Policies;

public static class AddressPolicy
{
    private const int MinLength = 3;
    private const int MaxLength = 200;

    private const string Field = "Address";
    private const string AllowedPattern = "^[^<>]*$";

    public static Result Validate(string? value)
    {
        var notEmpty = Guard.AgainstNullOrEmpty(value, ErrorResults.Required(Field));
        if (notEmpty.IsFailure) return notEmpty;

        var trimmed = value!.Trim();

        var range = Guard.AgainstStringRange(trimmed, MinLength, MaxLength,
            ErrorResults.InvalidLength(Field, MinLength, MaxLength));

        if (range.IsFailure) return range;

        return Guard.AgainstRegex(trimmed, AllowedPattern, ErrorResults.InvalidFormat(Field));
    }
}