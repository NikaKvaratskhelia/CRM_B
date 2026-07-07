using CRM_B.Application.Abstractions.Localization;
using CRM_B.Application.Common.Validation;
using FluentValidation;

namespace CRM_B.Application.Features.Users.EditProfile;

public sealed class EditProfileValidator : AbstractValidator<EditProfileCommand>
{
    public EditProfileValidator(IErrorLocalizer t, TimeProvider time)
    {
        RuleFor(x => x.FullName)
            .ValidFullName(t)
            .When(x => !string.IsNullOrWhiteSpace(x.FullName));

        RuleFor(x => x.PhoneNumber)
            .ValidPhoneNumber(t)
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x.Address)
            .ValidAddress(t)
            .When(x => !string.IsNullOrWhiteSpace(x.Address));

        RuleFor(x => x.DateOfBirth).ValidDateOfBirth(t, time);
    }
}