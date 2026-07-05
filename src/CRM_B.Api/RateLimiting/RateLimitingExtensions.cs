using Microsoft.AspNetCore.RateLimiting;

namespace CRM_B.Api.RateLimiting;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddAppRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            AddFixedWindow(options, RateLimitPolicies.AuthLogin, permits: 5);
            AddFixedWindow(options, RateLimitPolicies.AuthRefresh, permits: 30);
            AddFixedWindow(options, RateLimitPolicies.AuthRegister, permits: 5);
            AddFixedWindow(options, RateLimitPolicies.AuthForgotPassword, permits: 5);
            AddFixedWindow(options, RateLimitPolicies.AuthVerifyEmail, permits: 10);
            AddFixedWindow(options, RateLimitPolicies.AuthResendVerification, permits: 3);
            AddFixedWindow(options, RateLimitPolicies.GraphQL, permits: 100);
        });

        return services;
    }

    private static void AddFixedWindow(RateLimiterOptions options, string policy, int permits)
        => options.AddFixedWindowLimiter(policy, o =>
        {
            o.Window = TimeSpan.FromMinutes(1);
            o.PermitLimit = permits;
            o.QueueLimit = 0;
        });
}