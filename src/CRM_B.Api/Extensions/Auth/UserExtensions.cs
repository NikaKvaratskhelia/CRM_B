using System.Security.Claims;
using CRM_B.Domain.Aggregates.Users.Identifiers;
using Microsoft.AspNetCore.Mvc;

namespace CRM_B.Api.Extensions.Auth;

public static class UserExtensions
{
    public static UserId GetUserId(this ControllerBase controller) =>
        ParseClaim(controller.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

    public static UserId GetUserId(this IHttpContextAccessor httpContextAccessor) =>
        ParseClaim(httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

    private static UserId ParseClaim(string? value)
    {
        if (string.IsNullOrEmpty(value) || !Guid.TryParse(value, out var guid))
            throw new InvalidOperationException(
                "JWT subject claim is missing or malformed — authentication pipeline configuration bug.");

        return UserId.From(guid);
    }
}