namespace CRM_B.Api.RateLimiting;

public static class RateLimitPolicies
{
    public const string AuthLogin = "auth-login";
    public const string AuthRefresh = "auth-refresh";
    public const string AuthRegister = "auth-register";
    public const string AuthForgotPassword = "auth-forgot-password";
    public const string AuthVerifyEmail = "auth-verify-email";
    public const string AuthResendVerification = "auth-resend-verification";
    public const string GraphQL = "graphql";
}