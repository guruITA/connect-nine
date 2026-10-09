using Connect.Domain.Common;

namespace Connect.Domain.Users;

public sealed class Profile : Entity
{
    public Guid AccountId { get; set; }

    public required Account Account { get; set; }
}
