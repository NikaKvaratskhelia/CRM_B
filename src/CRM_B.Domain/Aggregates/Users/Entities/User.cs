using CRM_B.Domain.Aggregates.Auth.Entities;
using CRM_B.Domain.Aggregates.Auth.Enums;
using CRM_B.Domain.Aggregates.Auth.Events;
using CRM_B.Domain.Aggregates.Auth.Policies;
using CRM_B.Domain.Aggregates.Users.Enums;
using CRM_B.Domain.Aggregates.Users.Identifiers;
using CRM_B.Domain.Aggregates.Users.Policies;
using CRM_B.Domain.Aggregates.Users.ValueObjects;
using CRM_B.Domain.Kernel.Entities;
using CRM_B.Domain.Kernel.Events;
using CRM_B.Domain.Kernel.Guards;
using CRM_B.Domain.Kernel.Results;
using CRM_B.Domain.Kernel.Results.Errors;
using CRM_B.Domain.Kernel.Results.Extensions;

namespace CRM_B.Domain.Aggregates.Users.Entities;

public sealed class User : AggregateRoot<UserId>
{
    private readonly List<Verification> _verifications = new();

    private User()
    {
    }

    private User(string fullName, Email email, string passwordHash)
    {
        AuthProvider = AuthProvider.Password;
        FullName = fullName.Trim();
        PasswordHash = passwordHash;
        Email = email;
    }

    private User(string fullName, Email email, AuthProvider provider, string externalId)
    {
        FullName = fullName.Trim();
        ExternalId = externalId;
        AuthProvider = provider;
        IsVerified = true;
        Email = email;
    }

    public string FullName { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public string? PasswordHash { get; private set; }

    public UserRole Role { get; private set; } = UserRole.User;
    public bool IsVerified { get; private set; }

    public AuthProvider AuthProvider { get; private set; } = AuthProvider.Password;
    public string? ExternalId { get; private set; }

    public Language Language { get; private set; } = Language.Ka;

    public AccountLockout Lockout { get; private set; } = AccountLockout.Initial;
    public IReadOnlyCollection<Verification> Verifications => _verifications.AsReadOnly();

    public static Result<User> Create(string fullName, string email, string passwordHash) =>
        FullNamePolicy.Validate(fullName)
            .Bind(() => Email.Create(email))
            .Map(emailVo =>
            {
                var user = new User(fullName, emailVo, passwordHash);
                user.Raise(new UserRegisteredEvent(user.Id, user.Email.Value, user.FullName));
                return user;
            });

    public static Result<User>
        CreateExternal(string fullName, string email, AuthProvider provider, string externalId) =>
        FullNamePolicy.Validate(fullName)
            .Bind(() => Guard.AgainstNullOrEmpty(externalId, ErrorResults.Required("ExternalId")))
            .Bind(() => Email.Create(email))
            .Map(emailVo =>
            {
                var user = new User(fullName, emailVo, provider, externalId);
                user.Raise(new UserRegisteredEvent(user.Id, user.Email.Value, user.FullName));
                return user;
            });

    public void SetPasswordHash(string newHash) => PasswordHash = newHash;

    public void ResetPassword(string newHash)
    {
        PasswordHash = newHash;
        Lockout = Lockout.Clear();
        Raise(new PasswordResetCompletedEvent(Id, Email.Value, FullName, Language));
    }

    public Result Rename(string fullName) =>
        FullNamePolicy.Validate(fullName).Bind(() =>
        {
            FullName = fullName.Trim();
            return Result.Success();
        });

    public void Verify()
    {
        if (IsVerified) return;

        IsVerified = true;
        Raise(new UserVerifiedEvent(Id, Email.Value, FullName, Language));
    }

    public void RecordFailedLogin(DateTime now) => Lockout = Lockout.RecordFailure(now);
    public void RecordSuccessfulLogin(DateTime now) => Lockout = Lockout.RecordSuccess(now);

    public void ChangeLanguage(Language language) => Language = language;

    public void CreateVerification(VerificationType type, DateTime now)
    {
        var expiresAt = now.Add(VerificationLifetimes.Of(type));
        var verification = Verification.Create(Id, type, expiresAt);
        _verifications.Add(verification);

        Raise(type switch
        {
            VerificationType.Email
                => new EmailVerificationRequestedEvent(
                    Id, Email.Value, FullName, verification.Id,
                    verification.Otp, verification.Token, verification.ExpiresAt, Language),
            VerificationType.Password
                => (IDomainEvent)new PasswordResetRequestedEvent(
                    Id, Email.Value, FullName, verification.Id,
                    verification.Token, verification.ExpiresAt, Language),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported verification type."),
        });
    }
}