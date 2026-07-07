using CRM_B.Application.Features.Auth.EmailVerification.Resend;
using Riok.Mapperly.Abstractions;

namespace CRM_B.Api.Contracts.Auth;

public sealed record ResendVerificationRequest(string Email);

[Mapper]
public static partial class ResendVerificationMapper
{
    public static partial ResendEmailVerificationCommand ToCommand(this ResendVerificationRequest req);
}