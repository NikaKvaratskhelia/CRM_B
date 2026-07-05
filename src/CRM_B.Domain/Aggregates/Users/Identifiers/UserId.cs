using CRM_B.Domain.Kernel.Entities;

namespace CRM_B.Domain.Aggregates.Users.Identifiers;

public readonly record struct UserId(Guid Value) : IEntityId<UserId>
{
    public static UserId New() => new(Guid.NewGuid());
    public static UserId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}