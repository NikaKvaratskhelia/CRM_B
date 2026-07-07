using CRM_B.Application.Abstractions.Observability;
using CRM_B.Domain.Kernel.Entities;
using CRM_B.Domain.Kernel.Events;
using CRM_B.Infrastructure.Jobs.Outbox.Processing;
using CRM_B.Infrastructure.Jobs.Outbox.Serialization;
using CRM_B.Infrastructure.Persistence.Outbox;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace CRM_B.Infrastructure.Persistence.Interceptors;

internal sealed class OutboxInterceptor(
    ILogger<OutboxInterceptor> logger,
    DomainEventTypeRegistry registry,
    IBackgroundJobClient jobs,
    ICorrelationContext correlation,
    TimeProvider time
) : SaveChangesInterceptor
{
    private readonly List<Guid> _pendingMessageIds = [];

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        Stage(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
    {
        Stage(eventData.Context);
        return base.SavingChangesAsync(eventData, result, ct);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        Dispatch();
        return base.SavedChanges(eventData, result);
    }

    public override ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData, int result, CancellationToken ct = default)
    {
        Dispatch();
        return base.SavedChangesAsync(eventData, result, ct);
    }

    public override void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        _pendingMessageIds.Clear();
        base.SaveChangesFailed(eventData);
    }

    public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken ct = default)
    {
        _pendingMessageIds.Clear();
        return base.SaveChangesFailedAsync(eventData, ct);
    }

    private void Stage(DbContext? context)
    {
        _pendingMessageIds.Clear();
        if (context is null) return;

        var correlationId = correlation.CurrentId;
        var now = time.GetUtcNow().UtcDateTime;

        var aggregates = context.ChangeTracker.Entries<IAggregateRoot>()
            .Select(e => e.Entity).Where(a => a.Events.Count > 0).ToList();

        foreach (var aggregate in aggregates)
        {
            foreach (var @event in aggregate.Events)
            {
                var carrier = @event is IHasCorrelationId hasCorrelation
                    ? hasCorrelation.WithCorrelationId(correlationId)
                    : @event;

                var payload = OutboxEventSerializer.Serialize(carrier);
                var message = OutboxMessage.Create(registry.NameOf(@event), payload, correlationId, now);

                context.Add(message);
                _pendingMessageIds.Add(message.Id);
            }

            aggregate.ClearEvents();
        }
    }

    private void Dispatch()
    {
        foreach (var id in _pendingMessageIds)
        {
            try
            {
                jobs.Enqueue<OutboxProcessor>(p => p.ProcessOneAsync(id, CancellationToken.None));
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex,
                    "Failed to enqueue outbox message {Id}; safety-net poller will pick it up", id);
            }
        }

        _pendingMessageIds.Clear();
    }
}