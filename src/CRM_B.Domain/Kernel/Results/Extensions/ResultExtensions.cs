using CRM_B.Domain.Kernel.Results.Errors;

namespace CRM_B.Domain.Kernel.Results.Extensions;

public static class ResultExtensions
{
    public static Result ToResult<T>(this Result<T> result)
        => result.IsSuccess ? Result.Success() : Result.Failure(result.Error!);

    public static Result<T> ToFailure<T>(this Result result)
        => Result<T>.Failure(result.Error!);

    public static Result<TOut> ToFailure<TIn, TOut>(this Result<TIn> result)
        => Result<TOut>.Failure(result.Error!);

    public static Result<T> EnsureFound<T>(this T? value, ErrorResults error) where T : class
        => value is not null ? Result<T>.Success(value) : Result<T>.Failure(error);

    public static Result FirstFailureOrSuccess(params Result[] results)
    {
        foreach (var r in results)
            if (r.IsFailure)
                return r;

        return Result.Success();
    }

    public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, ErrorResults error)
    {
        if (result.IsFailure) return result;
        return predicate(result.Value!) ? result : Result<T>.Failure(error);
    }

    public static async Task<Result<T>> EnsureAsync<T>(this Task<Result<T>> task, Func<T, bool> predicate,
        ErrorResults error)
        => (await task).Ensure(predicate, error);

    public static Result<TOut> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> next)
        => result.IsSuccess ? next(result.Value!) : Result<TOut>.Failure(result.Error!);

    public static Result Bind<T>(this Result<T> result, Func<T, Result> next)
        => result.IsSuccess ? next(result.Value!) : Result.Failure(result.Error!);

    public static Result Bind(this Result result, Func<Result> next)
        => result.IsSuccess ? next() : result;

    public static Result<T> Bind<T>(this Result result, Func<Result<T>> next)
        => result.IsSuccess ? next() : Result<T>.Failure(result.Error!);

    public static async Task<Result<TOut>> BindAsync<TIn, TOut>(this Result<TIn> result,
        Func<TIn, Task<Result<TOut>>> next)
        => result.IsSuccess ? await next(result.Value!) : Result<TOut>.Failure(result.Error!);

    public static async Task<Result> BindAsync<T>(this Result<T> result, Func<T, Task<Result>> next)
        => result.IsSuccess ? await next(result.Value!) : Result.Failure(result.Error!);

    public static async Task<Result<TOut>> BindAsync<TIn, TOut>(this Task<Result<TIn>> task,
        Func<TIn, Task<Result<TOut>>> next)
    {
        var r = await task;
        return r.IsSuccess ? await next(r.Value!) : Result<TOut>.Failure(r.Error!);
    }

    public static async Task<Result<TOut>> BindAsync<TIn, TOut>(this Task<Result<TIn>> task,
        Func<TIn, Result<TOut>> next)
        => (await task).Bind(next);

    public static async Task<Result> BindAsync<T>(this Task<Result<T>> task, Func<T, Task<Result>> next)
    {
        var r = await task;
        return r.IsSuccess ? await next(r.Value!) : Result.Failure(r.Error!);
    }

    public static async Task<Result> BindAsync<T>(this Task<Result<T>> task, Func<T, Result> next)
        => (await task).Bind(next);

    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> map)
        => result.IsSuccess ? Result<TOut>.Success(map(result.Value!)) : Result<TOut>.Failure(result.Error!);

    public static async Task<Result<TOut>> MapAsync<TIn, TOut>(this Task<Result<TIn>> task, Func<TIn, TOut> map)
        => (await task).Map(map);

    public static Result<T> Tap<T>(this Result<T> result, Action<T> action)
    {
        if (result.IsSuccess) action(result.Value!);
        return result;
    }

    public static async Task<Result<T>> TapAsync<T>(this Result<T> result, Func<T, Task> action)
    {
        if (result.IsSuccess) await action(result.Value!);
        return result;
    }

    public static async Task<Result<T>> TapAsync<T>(this Task<Result<T>> task, Func<T, Task> action)
    {
        var r = await task;
        if (r.IsSuccess) await action(r.Value!);
        return r;
    }

    public static TOut Match<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> onSuccess,
        Func<ErrorResults, TOut> onFailure)
        => result.IsSuccess ? onSuccess(result.Value!) : onFailure(result.Error!);

    public static async Task OrThrow(this Task<Result> task)
    {
        var result = await task.ConfigureAwait(false);

        if (result.IsFailure)
        {
            var error = result.Error!;

            var message = error.Args.Length > 0 ? $"{error.Code}: {string.Join(", ", error.Args)}" : error.Code;

            throw new InvalidOperationException(message);
        }
    }
}