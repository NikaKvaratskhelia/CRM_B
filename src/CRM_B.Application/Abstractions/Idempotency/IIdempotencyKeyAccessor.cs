namespace CRM_B.Application.Abstractions.Idempotency;

public interface IIdempotencyKeyAccessor
{
    string? Current { get; }
}