using CRM_B.Application.Features.Auth.Login.Password;
using Riok.Mapperly.Abstractions;

namespace CRM_B.Api.Contracts.Auth;

public sealed record LoginRequest(string Email, string Password);

[Mapper]
public static partial class LoginMapper
{
    public static partial LoginCommand ToCommand(this LoginRequest req);
}