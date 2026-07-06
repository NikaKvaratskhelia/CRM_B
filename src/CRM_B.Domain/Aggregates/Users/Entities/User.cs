using CRM_B.Domain.Aggregates.Auth.Entities;
using CRM_B.Domain.Aggregates.Auth.Enums;
using CRM_B.Domain.Aggregates.Auth.Events;
using CRM_B.Domain.Aggregates.Auth.Policies;
using CRM_B.Domain.Aggregates.Users.Enums;
using CRM_B.Domain.Aggregates.Users.Identifiers;
using CRM_B.Domain.Aggregates.Users.Policies;
using CRM_B.Domain.Kernel.Entities;
using CRM_B.Domain.Kernel.Events;
using CRM_B.Domain.Kernel.Results;
using CRM_B.Domain.Kernel.Results.Extensions;
using CRM_B.Domain.ValueObjects;

namespace CRM_B.Domain.Aggregates.Users.Entities;

public sealed class User : AggregateRoot<UserId>
{
    private readonly List<Verification> _verifications = new();

    private User()
    {
    }

    private User(string fullName, Email email, string passwordHash)
    {
        FullName = fullName.Trim();
        PasswordHash = passwordHash;
        Email = email;
    }

    public string FullName { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public string? PasswordHash { get; private set; }

    public UserRole Role { get; private set; } = UserRole.User;
    public bool IsVerified { get; private set; }
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
                    verification.Otp, verification.Token, verification.ExpiresAt),
            VerificationType.Password
                => (IDomainEvent)new PasswordResetRequestedEvent(
                    Id, Email.Value, FullName, verification.Id,
                    verification.Token, verification.ExpiresAt),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported verification type."),
        });
    }
}