using CRM_B.Application.Abstractions.Localization;
using CRM_B.Application.Abstractions.Persistence;
using CRM_B.Application.Common.Validation;
using CRM_B.Domain.Aggregates.Users.Errors;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CRM_B.Application.Features.Auth.Register;

public sealed class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator(IDataContext db, IErrorLocalizer t)
    {
        RuleFor(x => x.FullName).ValidFullName(t);

        RuleFor(x => x.Email)
            .ValidEmail(t)
            .MustAsync(async (email, ct) =>
            {
                var normalized = email!.Trim().ToLowerInvariant();
                return !await db.Users.AnyAsync(u => u.Email.Value == normalized, ct);
            })
            .WithMessage(_ => t[UserErrors.EmailAlreadyRegistered.Code]);

        RuleFor(x => x.Password).ValidPassword(t);
    }
}