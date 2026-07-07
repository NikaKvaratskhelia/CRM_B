using CRM_B.Application.Abstractions.Localization;
using CRM_B.Application.Common.Validation;
using FluentValidation;

namespace CRM_B.Application.Features.Auth.Session.Refresh;

public sealed class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator(IErrorLocalizer t)
    {
        RuleFor(x => x.RefreshToken).RequiredField(t, "RefreshToken");
    }
}