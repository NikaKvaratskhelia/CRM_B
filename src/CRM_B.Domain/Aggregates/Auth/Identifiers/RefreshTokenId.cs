using CRM_B.Domain.Kernel.Entities;

namespace CRM_B.Domain.Aggregates.Auth.Identifiers;

public readonly record struct RefreshTokenId(Guid Value) : IEntityId<RefreshTokenId>
{
    public static RefreshTokenId New() => new(Guid.NewGuid());
    public static RefreshTokenId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}