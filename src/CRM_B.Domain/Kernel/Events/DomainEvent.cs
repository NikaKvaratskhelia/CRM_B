namespace CRM_B.Domain.Kernel.Events;

public abstract record DomainEvent : IDomainEvent, IHasCorrelationId
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    public string CorrelationId { get; init; } = "";

    public IDomainEvent WithCorrelationId(string correlationId) =>
        this with { CorrelationId = correlationId };
}