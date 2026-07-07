using CRM_B.Application.Features.Auth.PasswordReset.Apply;
using Riok.Mapperly.Abstractions;

namespace CRM_B.Api.Contracts.Auth;

public sealed record ResetPasswordRequest(string Token, string Password);

[Mapper]
public static partial class ResetPasswordMapper
{
    public static partial ResetPasswordCommand ToCommand(this ResetPasswordRequest req);
}