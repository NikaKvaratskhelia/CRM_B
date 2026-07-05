namespace CRM_B.Domain.Kernel.Entities;

public abstract class Entity<TId> : IAuditable where TId : IEntityId<TId>
{
    public TId Id { get; protected set; } = TId.New();

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    DateTime IAuditable.CreatedAt
    {
        get => CreatedAt;
        set => CreatedAt = value;
    }

    DateTime? IAuditable.UpdatedAt
    {
        get => UpdatedAt;
        set => UpdatedAt = value;
    }
}

public interface IAuditable
{
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
}

public interface IEntityId<TSelf> where TSelf : IEntityId<TSelf>
{
    Guid Value { get; }
    static abstract TSelf New();
    static abstract TSelf From(Guid value);
}