using CRM_B.Application.Abstractions.Messaging;
using CRM_B.Application.Contracts.Auth;

namespace CRM_B.Application.Features.Auth.Login.Password;

public sealed record LoginCommand(
    string Email,
    string Password
) : ICommand<LoginOutcome>;