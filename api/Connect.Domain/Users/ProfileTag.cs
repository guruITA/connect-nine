using Connect.Domain.Common;
using Connect.Domain.Tags;

namespace Connect.Domain.Users;

// Which profile has which tag. The link goes with the profile, the tag itself stays.
public sealed class ProfileTag : Entity
{
    public Guid ProfileId { get; set; }

    public Guid TagId { get; set; }

    public Tag? Tag { get; set; }
}
