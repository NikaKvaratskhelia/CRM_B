using CRM_B.Domain.Aggregates.Users.Identifiers;
using CRM_B.Domain.Kernel.Events;

namespace CRM_B.Domain.Aggregates.Auth.Events;

[EventType("user.registered.v1")]
public sealed record UserRegisteredEvent(
    UserId UserId,
    string Email,
    string FullName
) : DomainEvent;