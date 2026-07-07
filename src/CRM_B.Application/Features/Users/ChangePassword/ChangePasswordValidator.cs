using CRM_B.Application.Abstractions.Localization;
using CRM_B.Application.Common.Validation;
using FluentValidation;

namespace CRM_B.Application.Features.Users.ChangePassword;

public sealed class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordValidator(IErrorLocalizer t)
    {
        RuleFor(x => x.OldPassword).RequiredField(t, "Old password");
        RuleFor(x => x.NewPassword).ValidPassword(t);
    }
}