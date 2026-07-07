using CRM_B.Domain.Aggregates.Users.Policies;
using CRM_B.Domain.Kernel.Models;

namespace CRM_B.Domain.Aggregates.Users.ValueObjects;

public sealed class AccountLockout : ValueObject
{
    private AccountLockout()
    {
    }

    private AccountLockout(int failedAttempts, DateTime? lockedUntil, DateTime? lastLoginAt)
    {
        FailedAttempts = failedAttempts;
        LockedUntil = lockedUntil;
        LastLoginAt = lastLoginAt;
    }

    public int FailedAttempts { get; private set; }
    public DateTime? LockedUntil { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    public static AccountLockout Initial { get; } = new(0, null, null);

    public bool IsLockedAt(DateTime now) => LockedUntil.HasValue && LockedUntil.Value > now;

    public AccountLockout RecordFailure(DateTime now)
    {
        var attempts = FailedAttempts + 1;

        var lockedUntil = attempts >= AccountLockoutPolicy.MaxFailedAttempts
            ? now.Add(AccountLockoutPolicy.LockoutDuration)
            : LockedUntil;

        return new AccountLockout(attempts, lockedUntil, LastLoginAt);
    }

    public AccountLockout RecordSuccess(DateTime now) => new(0, null, now);

    public AccountLockout Clear() => new(0, null, LastLoginAt);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FailedAttempts;
        yield return LockedUntil;
        yield return LastLoginAt;
    }
}