using CRM_B.Application.Abstractions.Localization;
using CRM_B.Application.Common.Validation;
using FluentValidation;

namespace CRM_B.Application.Features.Auth.EmailVerification.Resend;

public sealed class ResendEmailVerificationValidator : AbstractValidator<ResendEmailVerificationCommand>
{
    public ResendEmailVerificationValidator(IErrorLocalizer t)
    {
        RuleFor(x => x.Email).ValidEmail(t);
    }
}