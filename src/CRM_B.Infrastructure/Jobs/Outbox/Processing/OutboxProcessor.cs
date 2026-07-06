using CRM_B.Domain.Kernel.Events;
using CRM_B.Infrastructure.Jobs.Outbox.Options;
using CRM_B.Infrastructure.Jobs.Outbox.Serialization;
using CRM_B.Infrastructure.Persistence.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CRM_B.Infrastructure.Jobs.Outbox.Processing;

public sealed class OutboxProcessor(
    DataContext db,
    IPublisher publisher,
    OutboxOptions options,
    DomainEventTypeRegistry registry,
    TimeProvider time,
    ILogger<OutboxProcessor> logger)
{
    public async Task ProcessOneAsync(Guid messageId, CancellationToken ct)
    {
        await using var tx = await db.Database.BeginTransactionAsync(ct);

        var message = await db.OutboxMessages.FromSqlRaw(
            """
            SELECT * FROM outbox_messages
            WHERE id = {0} AND processed_on IS NULL
            FOR UPDATE SKIP LOCKED
            """, messageId).FirstOrDefaultAsync(ct);

        if (message is null) return;

        using var scope = logger.BeginScope(new Dictionary<string, object?>
        {
            ["CorrelationId"] = message.CorrelationId,
            ["OutboxMessageId"] = message.Id,
            ["OutboxMessageType"] = message.Type,
        });

        Exception? failure = null;

        try
        {
            var type = registry.Resolve(message.Type);
            var @event = (IDomainEvent)OutboxEventSerializer.Deserialize(message.Payload, type);

            await publisher.Publish(@event, ct);
            message.MarkProcessed(time.GetUtcNow().UtcDateTime);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Outbox dispatch failed for message {Id} (type {Type})", message.Id, message.Type);
            message.MarkFailed(ex.Message);
            failure = ex;
        }

        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        if (failure is not null) throw failure;
    }

    public async Task ProcessPendingAsync(CancellationToken ct)
    {
        var cutoff = time.GetUtcNow().UtcDateTime - options.StragglerThreshold;

        var stragglerIds = await db.OutboxMessages
            .Where(m => m.ProcessedOn == null && m.RetryCount < options.MaxRetries && m.OccurredOn < cutoff)
            .OrderBy(m => m.OccurredOn)
            .Take(options.BatchSize)
            .Select(m => m.Id)
            .ToListAsync(ct);

        if (stragglerIds.Count == 0) return;

        logger.LogWarning("Outbox safety-net picked up {Count} straggler(s)", stragglerIds.Count);

        foreach (var id in stragglerIds)
        {
            try
            {
                await ProcessOneAsync(id, ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Outbox safety-net failed to process message {Id}", id);
            }
            finally
            {
                db.ChangeTracker.Clear();
            }
        }
    }
}