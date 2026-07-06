using CRM_B.Domain.Kernel.Guards;
using CRM_B.Domain.Kernel.Results;
using CRM_B.Domain.Kernel.Results.Errors;

namespace CRM_B.Domain.Aggregates.Auth.Policies;

public static class VerificationCodePolicy
{
    private const string Field = "Verification code";
    private const string Pattern = @"^\d{6}$";

    public static Result Validate(string? value)
    {
        var notEmpty = Guard.AgainstNullOrEmpty(value, ErrorResults.Required(Field));
        if (notEmpty.IsFailure) return notEmpty;

        return Guard.AgainstRegex(value!, Pattern, ErrorResults.InvalidFormat(Field));
    }
}