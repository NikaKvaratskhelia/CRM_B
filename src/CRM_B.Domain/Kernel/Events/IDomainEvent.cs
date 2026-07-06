using MediatR;

namespace CRM_B.Domain.Kernel.Events;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}