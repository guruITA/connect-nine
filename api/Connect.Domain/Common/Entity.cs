namespace Connect.Domain.Common;

public abstract class Entity : IAuditable
{
    public Guid Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }
}
