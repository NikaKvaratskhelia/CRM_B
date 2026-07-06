using CRM_B.Domain.Kernel.Results.Errors;

namespace CRM_B.Domain.Aggregates.Auth.Errors;

public static class AuthErrors
{
    public static readonly ErrorResults InvalidCredentials = new("auth.invalid_credentials")
        { Kind = ErrorKind.Unauthorized };

    public static readonly ErrorResults AccountLocked = new("auth.account_locked") { Kind = ErrorKind.Forbidden };

    public static readonly ErrorResults InvalidRefreshToken = new("auth.invalid_refresh_token")
        { Kind = ErrorKind.Unauthorized };

    public static readonly ErrorResults AlreadyVerified = new("auth.already_verified") { Kind = ErrorKind.Conflict };

    public static readonly ErrorResults VerificationNotFound = new("auth.verification_not_found")
        { Kind = ErrorKind.NotFound };

    public static readonly ErrorResults VerificationCodeIncorrect = new("auth.verification_code_incorrect");

    public static readonly ErrorResults VerificationCodeExpired = new("auth.verification_code_expired")
        { Kind = ErrorKind.Unauthorized };

    public static readonly ErrorResults PasswordResetNotFound = new("auth.password_reset_not_found")
        { Kind = ErrorKind.NotFound };

    public static readonly ErrorResults PasswordResetExpired = new("auth.password_reset_expired")
        { Kind = ErrorKind.Unauthorized };

    public static ErrorResults EmailSendFailed(string reason) =>
        new("auth.email_send_failed", reason) { Kind = ErrorKind.Internal };
}