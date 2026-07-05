using CRM_B.Domain.Kernel.Results.Errors;

namespace CRM_B.Domain.Kernel.Results;

public class Result : IErrorResponse
{
    private static readonly Result SuccessInstance = new(true, null);

    protected Result(bool success, ErrorResults? error)
    {
        IsSuccess = success;
        Error = error;
    }

    public bool IsFailure => !IsSuccess;

    public virtual bool HasValue => false;

    public bool IsSuccess { get; }
    public ErrorResults? Error { get; }

    public static Result Success() => SuccessInstance;
    public static Result Failure(ErrorResults error) => new(false, error);
    public virtual object? GetValue() => null;
}

public sealed class Result<T> : Result
{
    private Result(bool success, T? value, ErrorResults? error) : base(success, error)
    {
        Value = value;
    }

    public T? Value { get; }

    public override bool HasValue => true;

    public static Result<T> Success(T value) => new(true, value, null);
    public static new Result<T> Failure(ErrorResults error) => new(false, default, error);
    public override object? GetValue() => Value;
}

public interface IErrorResponse
{
    bool IsSuccess { get; }
    ErrorResults? Error { get; }
}