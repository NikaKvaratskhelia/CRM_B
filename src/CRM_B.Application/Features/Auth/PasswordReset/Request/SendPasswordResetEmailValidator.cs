using CRM_B.Application.Abstractions.Localization;
using CRM_B.Application.Common.Validation;
using FluentValidation;

namespace CRM_B.Application.Features.Auth.PasswordReset.Request;

public sealed class SendPasswordResetEmailValidator : AbstractValidator<SendPasswordResetEmailCommand>
{
    public SendPasswordResetEmailValidator(IErrorLocalizer t)
    {
        RuleFor(x => x.Email).ValidEmail(t);
    }
}