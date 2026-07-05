namespace CRM_B.Application.Abstractions.Idempotency;

public interface IIdempotencyStore
{
    Task<IdempotencyEntry?> FindAsync(string key, CancellationToken ct);

    void Stage(string key, string requestHash, string responsePayload, DateTime expiresAt);

    Task<int> PruneAsync(CancellationToken ct);
}

public sealed record IdempotencyEntry(string RequestHash, string ResponsePayload);