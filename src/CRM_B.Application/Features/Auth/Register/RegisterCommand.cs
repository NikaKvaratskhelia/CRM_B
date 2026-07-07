using CRM_B.Application.Abstractions.Idempotency;
using CRM_B.Application.Abstractions.Messaging;

namespace CRM_B.Application.Features.Auth.Register;

public sealed record RegisterCommand(
    string FullName,
    string Email,
    string Password
) : ICommand, IIdempotentCommand;