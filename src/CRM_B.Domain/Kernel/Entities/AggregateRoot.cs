using CRM_B.Domain.Kernel.Events;

namespace CRM_B.Domain.Kernel.Entities;

public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot where TId : IEntityId<TId>
{
    private readonly List<IDomainEvent> _events = new();

    public IReadOnlyList<IDomainEvent> Events => _events;

    public void ClearEvents() => _events.Clear();

    protected void Raise(IDomainEvent @event) => _events.Add(@event);
}

public interface IAggregateRoot
{
    IReadOnlyList<IDomainEvent> Events { get; }
    void ClearEvents();
}