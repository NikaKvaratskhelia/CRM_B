using CRM_B.Application.Features.Auth.Login.Google;
using Riok.Mapperly.Abstractions;

namespace CRM_B.Api.Contracts.Auth;

public sealed record GoogleLoginRequest(string IdToken);

[Mapper]
public static partial class GoogleLoginMapper
{
    public static partial GoogleLoginCommand ToCommand(this GoogleLoginRequest req);
}