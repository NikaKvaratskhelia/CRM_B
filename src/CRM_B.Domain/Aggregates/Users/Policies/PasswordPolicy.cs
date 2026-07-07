using CRM_B.Domain.Kernel.Guards;
using CRM_B.Domain.Kernel.Results;
using CRM_B.Domain.Kernel.Results.Errors;

namespace CRM_B.Domain.Aggregates.Users.Policies;

public static class PasswordPolicy
{
    private const int MinLength = 8;
    private const int MaxLength = 128;

    private const string Field = "Password";

    public static Result Validate(string? value)
    {
        var notEmpty = Guard.AgainstNullOrEmpty(value, ErrorResults.Required(Field));
        if (notEmpty.IsFailure) return notEmpty;

        return Guard.AgainstStringRange(value!, MinLength, MaxLength,
            ErrorResults.InvalidLength(Field, MinLength, MaxLength));
    }

    public static Result ValidateForLogin(string? value)
    {
        var notEmpty = Guard.AgainstNullOrEmpty(value, ErrorResults.Required(Field));
        if (notEmpty.IsFailure) return notEmpty;

        return Guard.AgainstStringRange(value!, 1, MaxLength,
            ErrorResults.InvalidLength(Field, 1, MaxLength));
    }
}