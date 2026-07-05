using CRM_B.Application.Abstractions.Identity;
using Microsoft.AspNetCore.Authorization;

namespace CRM_B.Api.Hosting.Identity;

public sealed class PolicyEvaluator(
    IAuthorizationService authorization,
    IHttpContextAccessor accessor) : IPolicyEvaluator
{
    public async Task<bool> EvaluateAsync(string policy, CancellationToken ct)
    {
        var principal = accessor.HttpContext?.User;
        if (principal is null) return false;

        var result = await authorization.AuthorizeAsync(principal, resource: null, policy);
        return result.Succeeded;
    }
}