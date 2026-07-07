using CRM_B.Application.Abstractions.Localization;
using CRM_B.Application.Common.Validation;
using CRM_B.Domain.Kernel.Results.Errors;
using FluentValidation;

namespace CRM_B.Application.Features.Auth.PasswordReset.Apply;

public sealed class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator(IErrorLocalizer t)
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage(_ => t.Get(ErrorResults.Required("Token").Code, "Token"));

        RuleFor(x => x.Password).ValidPassword(t);
    }
}