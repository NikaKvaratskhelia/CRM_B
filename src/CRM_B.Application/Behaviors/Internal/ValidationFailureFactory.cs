using System.Reflection;
using CRM_B.Domain.Kernel.Results;
using CRM_B.Domain.Kernel.Results.Errors;

namespace CRM_B.Application.Behaviors.Internal;

internal static class ValidationFailureFactory<TResponse>
{
    private static readonly Func<ErrorResults, TResponse> Factory = BuildFactory();

    public static TResponse Create(ErrorResults error) => Factory(error);

    private static Func<ErrorResults, TResponse> BuildFactory()
    {
        if (typeof(TResponse) == typeof(Result))
            return err => (TResponse)(object)Result.Failure(err);

        if (typeof(TResponse).IsGenericType
            && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var failureMethod = typeof(TResponse)
                .GetMethod(
                    nameof(Result.Failure),
                    BindingFlags.Public | BindingFlags.Static,
                    [typeof(ErrorResults)])!;

            return err => (TResponse)failureMethod.Invoke(null, [err])!;
        }

        throw new InvalidOperationException(
            $"Cannot build a failure of type '{typeof(TResponse)}'. " +
            "Validated requests must return Result or Result<T>.");
    }
}