using CRM_B.Application.Abstractions.Idempotency;
using CRM_B.Application.Abstractions.Messaging;

namespace CRM_B.Application.Features.Auth.PasswordReset.Apply;

public sealed record ResetPasswordCommand(
    string Token,
    string Password
) : ICommand, IIdempotentCommand;