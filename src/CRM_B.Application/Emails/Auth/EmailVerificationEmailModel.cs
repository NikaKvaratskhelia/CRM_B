namespace CRM_B.Application.Emails.Auth;

public sealed record EmailVerificationEmailModel(
    string FullName,
    string Otp,
    string VerificationUrl);