using CRM_B.Application.Features.Users.EditProfile;
using CRM_B.Domain.Aggregates.Users.Identifiers;
using Riok.Mapperly.Abstractions;

namespace CRM_B.Api.Contracts.Users;

public sealed record EditProfileRequest(string? FullName, string? PhoneNumber, string? Address, DateTime? DateOfBirth);

[Mapper]
public static partial class EditProfileMapper
{
    public static partial EditProfileCommand ToCommand(this EditProfileRequest req, UserId userId);
}