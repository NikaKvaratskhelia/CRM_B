using CRM_B.Domain.Aggregates.Users.Identifiers;
using CRM_B.Domain.Kernel.Events;

namespace CRM_B.Domain.Aggregates.Auth.Events;

[EventType("user.password_reset_completed.v1")]
public sealed record PasswordResetCompletedEvent(
    UserId UserId,
    string Email,
    string FullName
) : DomainEvent;