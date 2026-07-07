using CRM_B.Domain.Aggregates.Users.Errors;
using CRM_B.Domain.Kernel.Guards;
using CRM_B.Domain.Kernel.Models;
using CRM_B.Domain.Kernel.Results;
using CRM_B.Domain.Kernel.Results.Extensions;

namespace CRM_B.Domain.Aggregates.Users.ValueObjects;

public sealed class DateOfBirth : ValueObject
{
    private const int MinAgeYears = 13;
    private const int MaxAgeYears = 120;

    private DateOfBirth(DateOnly value) => Value = value;

    public DateOnly Value { get; }

    public static Result<DateOfBirth> Create(DateTime value, DateTime now) =>
        Create(DateOnly.FromDateTime(value), DateOnly.FromDateTime(now));

    private static Result<DateOfBirth> Create(DateOnly value, DateOnly today)
    {
        if (value > today)
            return Result<DateOfBirth>.Failure(UserErrors.DateOfBirthInFuture);

        var age = CalculateAge(value, today);

        return Guard.AgainstOutOfRange(age, MinAgeYears, MaxAgeYears,
                UserErrors.AgeOutOfRange(MinAgeYears, MaxAgeYears))
            .Bind(() => Result<DateOfBirth>.Success(new DateOfBirth(value)));
    }

    public int AgeAt(DateTime now) => CalculateAge(Value, DateOnly.FromDateTime(now));

    private static int CalculateAge(DateOnly birth, DateOnly on)
    {
        var age = on.Year - birth.Year;
        if (birth > on.AddYears(-age)) age--;
        return age;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString("yyyy-MM-dd");
}