using CRM_B.Application.Abstractions.Messaging;
using CRM_B.Application.Contracts.Auth;

namespace CRM_B.Application.Features.Auth.Login.Google;

public sealed record GoogleLoginCommand(string IdToken) : ICommand<LoginOutcome>;