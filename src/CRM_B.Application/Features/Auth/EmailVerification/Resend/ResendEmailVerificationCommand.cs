using CRM_B.Application.Abstractions.Messaging;

namespace CRM_B.Application.Features.Auth.EmailVerification.Resend;

public sealed record ResendEmailVerificationCommand(
    string Email
) : ICommand;