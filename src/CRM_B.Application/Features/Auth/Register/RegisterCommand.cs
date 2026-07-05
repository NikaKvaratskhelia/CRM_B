using System;
using CRM_B.Application.Abstractions.Messaging;

namespace CRM_B.Application.Features.Auth.Register;

public record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName) : ICommand<Guid>;