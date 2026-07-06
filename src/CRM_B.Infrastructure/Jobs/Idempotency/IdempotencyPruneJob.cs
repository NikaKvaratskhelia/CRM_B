using CRM_B.Application.Abstractions.Idempotency;
using Microsoft.Extensions.Logging;

namespace CRM_B.Infrastructure.Jobs.Idempotency;

public sealed class IdempotencyPruneJob(IIdempotencyStore store, ILogger<IdempotencyPruneJob> logger)
{
    public async Task RunAsync(CancellationToken ct)
    {
        var deleted = await store.PruneAsync(ct);

        if (deleted > 0)
            logger.LogInformation("Idempotency prune removed {Count} expired key(s)", deleted);
    }
}