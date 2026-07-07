using CRM_B.Application.Abstractions.Localization;
using CRM_B.Application.Common.Validation;
using FluentValidation;

namespace CRM_B.Application.Features.Auth.EmailVerification.Verify;

public sealed class VerifyEmailValidator : AbstractValidator<VerifyEmailCommand>
{
    public VerifyEmailValidator(IErrorLocalizer t)
    {
        RuleFor(x => x.Email).ValidEmail(t);
        RuleFor(x => x.Otp).ValidVerificationCode(t);
    }
}