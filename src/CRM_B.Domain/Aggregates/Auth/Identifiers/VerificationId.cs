using CRM_B.Domain.Kernel.Entities;

namespace CRM_B.Domain.Aggregates.Auth.Identifiers;

public readonly record struct VerificationId(Guid Value) : IEntityId<VerificationId>
{
    public static VerificationId New() => new(Guid.NewGuid());
    public static VerificationId From(Guid value) => new(value);
    public override string ToString() => Value.ToString();
}