using CRM_B.Domain.Aggregates.Users.Enums;
using CRM_B.Domain.Aggregates.Users.Identifiers;
using CRM_B.Domain.Kernel.Events;

namespace CRM_B.Domain.Aggregates.Auth.Events;

[EventType("user.verified.v1")]
public sealed record UserVerifiedEvent(
    UserId UserId,
    string Email,
    string FullName,
    Language Language
) : DomainEvent;