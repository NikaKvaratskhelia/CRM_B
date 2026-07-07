using CRM_B.Application.Abstractions.Messaging;

namespace CRM_B.Application.Features.Auth.PasswordReset.Request;

public sealed record SendPasswordResetEmailCommand(string Email) : ICommand;