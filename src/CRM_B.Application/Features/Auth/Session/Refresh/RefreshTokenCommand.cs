using CRM_B.Application.Abstractions.Messaging;
using CRM_B.Application.Contracts.Auth;

namespace CRM_B.Application.Features.Auth.Session.Refresh;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<LoginOutcome>;