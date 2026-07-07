using CRM_B.Application.Abstractions.Messaging;
using CRM_B.Domain.Aggregates.Users.Identifiers;

namespace CRM_B.Application.Features.Users.EditProfile;

public sealed record EditProfileCommand(
    UserId UserId,
    string? FullName,
    string? PhoneNumber,
    string? Address,
    DateTime? DateOfBirth
) : ICommand;