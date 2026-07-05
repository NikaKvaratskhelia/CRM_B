using CRM_B.Application.Abstractions.Idempotency;
using CRM_B.Infrastructure.Persistence.Data;
using CRM_B.Infrastructure.Persistence.Idempotency.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM_B.Infrastructure.Persistence.Idempotency.Store;

public sealed class IdempotencyStore(DataContext db, TimeProvider time) : IIdempotencyStore
{
    public async Task<IdempotencyEntry?> FindAsync(string key, CancellationToken ct)
    {
        var now = time.GetUtcNow().UtcDateTime;

        return await db.IdempotencyKeys
            .AsNoTracking()
            .Where(k => k.Key == key && k.ExpiresAt > now)
            .Select(k => new IdempotencyEntry(k.RequestHash, k.ResponsePayload))
            .FirstOrDefaultAsync(ct);
    }

    public void Stage(string key, string requestHash, string responsePayload, DateTime expiresAt) =>
        db.IdempotencyKeys.Add(IdempotencyKey.Create(key, requestHash, responsePayload, time.GetUtcNow().UtcDateTime,
            expiresAt));

    public Task<int> PruneAsync(CancellationToken ct)
    {
        var now = time.GetUtcNow().UtcDateTime;
        return db.IdempotencyKeys.Where(k => k.ExpiresAt <= now).ExecuteDeleteAsync(ct);
    }
}