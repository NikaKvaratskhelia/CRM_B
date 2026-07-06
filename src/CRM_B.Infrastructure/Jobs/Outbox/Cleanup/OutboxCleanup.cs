using CRM_B.Infrastructure.Jobs.Outbox.Options;
using CRM_B.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CRM_B.Infrastructure.Jobs.Outbox.Cleanup;

public sealed class OutboxCleanup(
    DataContext db,
    OutboxOptions options,
    TimeProvider time,
    ILogger<OutboxCleanup> logger)
{
    public async Task RunAsync(CancellationToken ct)
    {
        var cutoff = time.GetUtcNow().UtcDateTime - options.RetentionPeriod;

        var deleted = await db.OutboxMessages
            .Where(m => m.ProcessedOn != null && m.ProcessedOn < cutoff)
            .ExecuteDeleteAsync(ct);

        if (deleted > 0)
            logger.LogInformation("Outbox cleanup removed {Count} processed message(s) older than {Cutoff:o}",
                deleted, cutoff);
    }
}