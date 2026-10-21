using Connect.Domain.Common;

namespace Connect.Domain.Users;

public sealed class Profile : Entity
{
    public Guid AccountId { get; set; }

    public required Account Account { get; set; }

    public required string FirstName { get; set; }

    public AgeBand AgeBand { get; set; }

    public required string Region { get; set; }

    public string? Bio { get; set; }
}
