using System.Text.RegularExpressions;
using CRM_B.Domain.Kernel.Results;
using CRM_B.Domain.Kernel.Results.Errors;

namespace CRM_B.Domain.Kernel.Guards;

public static class Guard
{
    public static Result AgainstNullOrEmpty(string? value, ErrorResults errors) =>
        string.IsNullOrWhiteSpace(value) ? Result.Failure(errors) : Result.Success();

    public static Result AgainstRegex(string value, string pattern, ErrorResults errors) =>
        !Regex.IsMatch(value, pattern)
            ? Result.Failure(errors)
            : Result.Success();

    public static Result AgainstStringRange(string value, int min, int max, ErrorResults errors)
    {
        if (string.IsNullOrWhiteSpace(value)) return Result.Failure(errors);

        return (value.Length < min || value.Length > max)
            ? Result.Failure(errors)
            : Result.Success();
    }

    public static Result AgainstOutOfRange(int value, int min, int max, ErrorResults errors)
    {
        return (value < min || value > max)
            ? Result.Failure(errors)
            : Result.Success();
    }


    public static Result AgainstEqualsStringLength(string value, int exactLength, ErrorResults errors)
    {
        if (string.IsNullOrWhiteSpace(value)) return Result.Failure(errors);

        return (value.Length != exactLength)
            ? Result.Failure(errors)
            : Result.Success();
    }
}