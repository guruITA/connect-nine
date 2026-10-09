using Connect.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace Connect.Domain.Users;

public sealed class Account : IdentityUser<Guid>, IAuditable
{
    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public Profile? Profile { get; set; }
}
