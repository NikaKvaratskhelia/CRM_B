using CRM_B.Application.Abstractions.Messaging;
using CRM_B.Domain.Aggregates.Users.Identifiers;

namespace CRM_B.Application.Features.Users.ChangePassword;

public sealed record ChangePasswordCommand(
    UserId UserId,
    string OldPassword,
    string NewPassword
) : ICommand;