using CRM_B.Domain.Aggregates.Auth.Identifiers;
using CRM_B.Domain.Aggregates.Users.Enums;
using CRM_B.Domain.Aggregates.Users.Identifiers;
using CRM_B.Domain.Kernel.Events;

namespace CRM_B.Domain.Aggregates.Auth.Events;

[EventType("user.password_reset_requested.v1")]
public sealed record PasswordResetRequestedEvent(
    UserId UserId,
    string Email,
    string FullName,
    VerificationId VerificationId,
    string Token,
    DateTime ExpiresAt,
    Language Language
) : DomainEvent;