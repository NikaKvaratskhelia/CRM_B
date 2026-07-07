namespace CRM_B.Application.Emails.Auth;

public sealed record ForgotPasswordEmailModel(
    string FullName,
    string Token,
    string ResetUrl);