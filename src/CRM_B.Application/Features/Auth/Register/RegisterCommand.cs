using CRM_B.Application.Abstractions.Messaging;
using CRM_B.Domain.Aggregates.Users.Identifiers;

namespace CRM_B.Application.Features.Auth.Register;

public record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName) : ICommand<UserId>;