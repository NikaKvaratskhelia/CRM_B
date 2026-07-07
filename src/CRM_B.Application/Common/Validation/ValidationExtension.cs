using CRM_B.Application.Abstractions.Localization;
using CRM_B.Domain.Aggregates.Auth.Policies;
using CRM_B.Domain.Aggregates.Users.Policies;
using CRM_B.Domain.Aggregates.Users.ValueObjects;
using CRM_B.Domain.Kernel.Results;
using CRM_B.Domain.Kernel.Results.Errors;
using CRM_B.Domain.Kernel.Results.Extensions;
using FluentValidation;

namespace CRM_B.Application.Common.Validation;

public static class ValidationExtensions
{
    public static IRuleBuilderOptions<T, string?> ValidEmail<T>(this IRuleBuilder<T, string?> rule, IErrorLocalizer t)
        => rule.SatisfiesPolicy(v => Email.Create(v).ToResult(), t);

    public static IRuleBuilderOptions<T, string?> ValidPassword<T>(this IRuleBuilder<T, string?> rule,
        IErrorLocalizer t)
        => rule.SatisfiesPolicy(PasswordPolicy.Validate, t);

    public static IRuleBuilderOptions<T, string?> ValidPasswordForLogin<T>(this IRuleBuilder<T, string?> rule,
        IErrorLocalizer t)
        => rule.SatisfiesPolicy(PasswordPolicy.ValidateForLogin, t);

    public static IRuleBuilderOptions<T, string?> ValidFullName<T>(this IRuleBuilder<T, string?> rule,
        IErrorLocalizer t)
        => rule.SatisfiesPolicy(FullNamePolicy.Validate, t);

    public static IRuleBuilderOptions<T, string?> ValidAddress<T>(this IRuleBuilder<T, string?> rule, IErrorLocalizer t)
        => rule.SatisfiesPolicy(AddressPolicy.Validate, t);

    public static IRuleBuilderOptions<T, string?> ValidPhoneNumber<T>(this IRuleBuilder<T, string?> rule,
        IErrorLocalizer t)
        => rule.SatisfiesPolicy(v => PhoneNumber.Create(v).ToResult(), t);

    public static IRuleBuilderOptions<T, string?> ValidVerificationCode<T>(this IRuleBuilder<T, string?> rule,
        IErrorLocalizer t)
        => rule.SatisfiesPolicy(VerificationCodePolicy.Validate, t);

    public static IRuleBuilderOptions<T, DateTime?> ValidDateOfBirth<T>(
        this IRuleBuilder<T, DateTime?> rule, IErrorLocalizer t, TimeProvider time)
        => rule.SatisfiesPolicy(
            v => v.HasValue
                ? DateOfBirth.Create(v.Value, time.GetUtcNow().UtcDateTime).ToResult()
                : Result.Success(),
            t);

    public static IRuleBuilderOptions<T, string?> RequiredField<T>(
        this IRuleBuilder<T, string?> rule, IErrorLocalizer t, string fieldLabel)
        => rule.NotEmpty().WithMessage(_ => t.Get(ErrorResults.Required(fieldLabel).Code, fieldLabel));

    private static IRuleBuilderOptions<T, TValue> SatisfiesPolicy<T, TValue>(
        this IRuleBuilder<T, TValue> rule,
        Func<TValue, Result> validate,
        IErrorLocalizer t)
    {
        return rule
            .Must(value => validate(value).IsSuccess)
            .WithMessage((_, value) =>
            {
                var error = validate(value).Error!;
                return t.Get(error.Code, error.Args);
            });
    }
}