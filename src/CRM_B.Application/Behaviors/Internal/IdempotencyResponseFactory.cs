using System.Reflection;
using System.Text.Json;
using CRM_B.Domain.Kernel.Results;
using CRM_B.Domain.Kernel.Results.Errors;

namespace CRM_B.Application.Behaviors.Internal;

internal static class IdempotencyResponseFactory<TResponse>
{
    private static readonly Type? ValueType = ResolveValueType();
    private static readonly Func<ErrorResults, TResponse> FailureFactory = BuildFailureFactory();
    private static readonly Func<object?, TResponse> SuccessFactory = BuildSuccessFactory();

    public static string Serialize(TResponse response)
    {
        if (ValueType is null) return "null";

        var value = ((Result)(object)response!).GetValue();
        return JsonSerializer.Serialize(value, ValueType);
    }

    public static TResponse Deserialize(string payload)
    {
        if (ValueType is null) return SuccessFactory(null);

        var value = JsonSerializer.Deserialize(payload, ValueType);
        return SuccessFactory(value);
    }

    public static TResponse Failure(ErrorResults error) => FailureFactory(error);

    private static Type? ResolveValueType() =>
        typeof(TResponse).IsGenericType
        && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>)
            ? typeof(TResponse).GetGenericArguments()[0]
            : null;

    private static Func<ErrorResults, TResponse> BuildFailureFactory()
    {
        if (typeof(TResponse) == typeof(Result))
            return err => (TResponse)(object)Result.Failure(err);

        if (ValueType is not null)
        {
            var failure = typeof(TResponse).GetMethod(
                nameof(Result.Failure),
                BindingFlags.Public | BindingFlags.Static,
                [typeof(ErrorResults)])!;
            return err => (TResponse)failure.Invoke(null, [err])!;
        }

        throw new InvalidOperationException(
            $"Cannot build a failure for '{typeof(TResponse)}'. " +
            "Idempotent commands must return Result or Result<T>.");
    }

    private static Func<object?, TResponse> BuildSuccessFactory()
    {
        if (typeof(TResponse) == typeof(Result))
            return _ => (TResponse)(object)Result.Success();

        if (ValueType is not null)
        {
            var success = typeof(TResponse).GetMethod(
                nameof(Result.Success),
                BindingFlags.Public | BindingFlags.Static,
                [ValueType])!;
            return value => (TResponse)success.Invoke(null, [value])!;
        }

        throw new InvalidOperationException(
            $"Cannot build a success for '{typeof(TResponse)}'. " +
            "Idempotent commands must return Result or Result<T>.");
    }
}