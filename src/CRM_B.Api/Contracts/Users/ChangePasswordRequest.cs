using CRM_B.Application.Features.Users.ChangePassword;
using CRM_B.Domain.Aggregates.Users.Identifiers;
using Riok.Mapperly.Abstractions;

namespace CRM_B.Api.Contracts.Users;

public sealed record ChangePasswordRequest(string OldPassword, string NewPassword);

[Mapper]
public static partial class ChangePasswordMapper
{
    public static partial ChangePasswordCommand ToCommand(this ChangePasswordRequest req, UserId userId);
}