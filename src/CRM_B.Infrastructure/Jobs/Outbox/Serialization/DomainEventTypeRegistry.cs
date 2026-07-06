using System.Reflection;
using CRM_B.Domain.Kernel.Events;

namespace CRM_B.Infrastructure.Jobs.Outbox.Serialization;

public sealed class DomainEventTypeRegistry
{
    private readonly IReadOnlyDictionary<Type, string> _idByType;
    private readonly IReadOnlyDictionary<string, Type> _typeById;

    public DomainEventTypeRegistry()
    {
        var events = typeof(IDomainEvent).Assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false } && typeof(IDomainEvent).IsAssignableFrom(t))
            .ToList();

        var unmarked = events.Where(t => t.GetCustomAttribute<EventTypeAttribute>() is null).ToList();

        if (unmarked.Count > 0)
        {
            throw new InvalidOperationException(
                $"Domain event(s) missing [EventType(\"...\")] attribute: " +
                string.Join(", ", unmarked.Select(t => t.Name)));
        }

        _typeById = events.ToDictionary(
            t => t.GetCustomAttribute<EventTypeAttribute>()!.Id,
            t => t);

        _idByType = events.ToDictionary(
            t => t,
            t => t.GetCustomAttribute<EventTypeAttribute>()!.Id);
    }

    public string NameOf(IDomainEvent @event) =>
        _idByType.TryGetValue(@event.GetType(), out var id)
            ? id
            : throw new InvalidOperationException(
                $"Domain event {@event.GetType().Name} is missing [EventType(\"...\")] attribute");

    public Type Resolve(string typeName) =>
        _typeById.TryGetValue(typeName, out var type)
            ? type
            : throw new InvalidOperationException($"Unknown domain event type id: {typeName}");
}