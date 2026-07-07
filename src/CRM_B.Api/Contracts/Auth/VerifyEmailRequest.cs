using CRM_B.Application.Features.Auth.EmailVerification.Verify;
using Riok.Mapperly.Abstractions;

namespace CRM_B.Api.Contracts.Auth;

public sealed record VerifyEmailRequest(string Email, string Otp);

[Mapper]
public static partial class VerifyEmailMapper
{
    public static partial VerifyEmailCommand ToCommand(this VerifyEmailRequest req);
}