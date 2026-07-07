using CRM_B.Application.Abstractions.Localization;
using CRM_B.Application.Common.Validation;
using FluentValidation;

namespace CRM_B.Application.Features.Auth.Login.Password;

public sealed class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator(IErrorLocalizer t)
    {
        RuleFor(x => x.Email).ValidEmail(t);
        RuleFor(x => x.Password).ValidPasswordForLogin(t);
    }
}