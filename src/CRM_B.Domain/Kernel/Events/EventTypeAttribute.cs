namespace CRM_B.Domain.Kernel.Events;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class EventTypeAttribute(string id) : Attribute
{
    public string Id { get; } = id;
}