using CRM_B.Domain.Aggregates.Users.Enums;
using Microsoft.AspNetCore.Authorization;

namespace CRM_B.Api.Hosting.Identity;

public static class Policies
{
    public const string AdminOnly = "AdminOnly";
    public const string SuperAdminOnly = "SuperAdminOnly";

    public static AuthorizationOptions AddAppPolicies(this AuthorizationOptions options)
    {
        options.AddPolicy(AdminOnly, p =>
            p.RequireRole(nameof(UserRole.Admin), nameof(UserRole.SuperAdmin)));

        options.AddPolicy(SuperAdminOnly, p =>
            p.RequireRole(nameof(UserRole.SuperAdmin)));

        return options;
    }
}