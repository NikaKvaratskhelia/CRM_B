using CRM_B.Domain.Aggregates.Users.Identifiers;

namespace CRM_B.Application.Abstractions.Identity;

public interface ICurrentUser
{
    UserId Id { get; }
    bool IsAuthenticated { get; }
}