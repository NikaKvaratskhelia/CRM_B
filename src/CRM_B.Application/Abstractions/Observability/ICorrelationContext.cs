namespace CRM_B.Application.Abstractions.Observability;

public interface ICorrelationContext
{
    string CurrentId { get; }
}