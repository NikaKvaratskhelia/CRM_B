using CRM_B.Domain.Aggregates.Auth.Identifiers;
using CRM_B.Domain.Aggregates.Users.Identifiers;
using CRM_B.Domain.Kernel.Events;

namespace CRM_B.Domain.Aggregates.Auth.Events;

[EventType("user.email_verification_requested.v1")]
public sealed record EmailVerificationRequestedEvent(
    UserId UserId,
    string Email,
    string FullName,
    VerificationId VerificationId,
    string Otp,
    string Token,
    DateTime ExpiresAt
) : DomainEvent;