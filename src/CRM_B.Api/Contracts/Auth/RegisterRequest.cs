using CRM_B.Application.Features.Auth.Register;
using Riok.Mapperly.Abstractions;

namespace CRM_B.Api.Contracts.Auth;

public sealed record RegisterRequest(
    string Email,
    string Password,
    string FullName);

[Mapper]
public static partial class RegisterMapper
{
    public static partial RegisterCommand ToCommand(this RegisterRequest req);
}