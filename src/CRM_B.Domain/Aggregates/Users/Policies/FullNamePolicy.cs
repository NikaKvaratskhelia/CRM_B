using CRM_B.Domain.Kernel.Guards;
using CRM_B.Domain.Kernel.Results;
using CRM_B.Domain.Kernel.Results.Errors;

namespace CRM_B.Domain.Aggregates.Users.Policies;

public static class FullNamePolicy
{
    private const string Field = "Full name";
    private const int MinLength = 2;
    private const int MaxLength = 100;

    public static Result Validate(string? value)
    {
        var notEmpty = Guard.AgainstNullOrEmpty(value, ErrorResults.Required(Field));
        if (notEmpty.IsFailure) return notEmpty;

        return Guard.AgainstStringRange(value!.Trim(), MinLength, MaxLength,
            ErrorResults.InvalidLength(Field, MinLength, MaxLength));
    }
}