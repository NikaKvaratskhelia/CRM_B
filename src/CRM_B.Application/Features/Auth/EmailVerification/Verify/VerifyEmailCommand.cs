using CRM_B.Application.Abstractions.Messaging;
using CRM_B.Application.Contracts.Auth;

namespace CRM_B.Application.Features.Auth.EmailVerification.Verify;

public sealed record VerifyEmailCommand(
    string Email,
    string Otp
) : ICommand<LoginOutcome>;