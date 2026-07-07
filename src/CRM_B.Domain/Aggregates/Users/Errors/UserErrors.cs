using CRM_B.Domain.Kernel.Results.Errors;

namespace CRM_B.Domain.Aggregates.Users.Errors;

public static class UserErrors
{
    public static readonly ErrorResults AccountExistsWithDifferentProvider =
        new("user.account_exists_with_different_provider") { Kind = ErrorKind.Conflict };

    public static readonly ErrorResults EmailAlreadyRegistered = new("user.email_already_registered")
        { Kind = ErrorKind.Conflict };

    public static readonly ErrorResults DateOfBirthInFuture = new("user.date_of_birth_in_future");
    public static ErrorResults AgeOutOfRange(int min, int max) => new("user.age_out_of_range", min, max);
}