using CRM_B.Application.Abstractions.Idempotency;

namespace CRM_B.Application.Behaviors.Internal;

internal static class IdempotencyMarker<TRequest>
{
    public static readonly bool IsIdempotent =
        typeof(IIdempotentCommand).IsAssignableFrom(typeof(TRequest));
}