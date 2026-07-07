namespace CRM_B.Domain.Aggregates.Users.Policies;

public static class AccountLockoutPolicy
{
    public const int MaxFailedAttempts = 5;
    public static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);
}