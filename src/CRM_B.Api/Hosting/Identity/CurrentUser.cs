using System.Security.Claims;
using CRM_B.Application.Abstractions.Identity;
using CRM_B.Domain.Aggregates.Users.Identifiers;

namespace CRM_B.Api.Hosting.Identity;

public sealed class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public bool IsAuthenticated =>
        accessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

    public UserId Id
    {
        get
        {
            var value = accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(value) || !Guid.TryParse(value, out var guid))
                throw new InvalidOperationException(
                    "Current user identity is unavailable — caller must check IsAuthenticated first.");

            return UserId.From(guid);
        }
    }
}