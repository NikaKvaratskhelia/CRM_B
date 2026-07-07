using CRM_B.Application.Abstractions.Messaging;

namespace CRM_B.Application.Features.Auth.Session.Logout;

public sealed record LogoutCommand(string RefreshToken) : ICommand;