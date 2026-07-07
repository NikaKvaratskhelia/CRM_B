using CRM_B.Application.Abstractions.Messaging;
using CRM_B.Domain.Aggregates.Users.Identifiers;

namespace CRM_B.Application.Features.Users.DeleteProfile;

public sealed record DeleteProfileCommand(UserId UserId) : ICommand;