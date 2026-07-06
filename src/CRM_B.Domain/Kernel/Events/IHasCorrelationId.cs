namespace CRM_B.Domain.Kernel.Events;

public interface IHasCorrelationId
{
    string CorrelationId { get; }

    IDomainEvent WithCorrelationId(string correlationId);
}