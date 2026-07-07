using CRM_B.Application.Features.Auth.PasswordReset.Request;
using Riok.Mapperly.Abstractions;

namespace CRM_B.Api.Contracts.Auth;

public sealed record ForgotPasswordRequest(string Email);

[Mapper]
public static partial class ForgotPasswordMapper
{
    public static partial SendPasswordResetEmailCommand ToCommand(this ForgotPasswordRequest req);
}