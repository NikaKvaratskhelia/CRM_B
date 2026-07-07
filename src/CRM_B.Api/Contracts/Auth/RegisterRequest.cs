using CRM_B.Application.Features.Auth.Register;
using Riok.Mapperly.Abstractions;

namespace CRM_B.Api.Contracts.Auth;

public sealed record RegisterRequest(string FullName, string Email, string Password);

[Mapper]
public static partial class RegisterMapper
{
    public static partial RegisterCommand ToCommand(this RegisterRequest req);
}