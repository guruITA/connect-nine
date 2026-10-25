namespace Connect.Domain.Common;

public abstract class Entity : IEntity, IAuditable
{
    public Guid Id { get; } = Guid.NewGuid();

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }
}
