using Connect.Domain.Common;
using Connect.Domain.ParentConsents;

namespace Connect.Domain.Users;

public sealed class Profile : Entity
{
    public Guid AccountId { get; set; }

    public required Account Account { get; set; }

    public required string FirstName { get; set; }

    public AgeBand AgeBand { get; set; }

    public required string Region { get; set; }

    public string? Bio { get; set; }

    public ParentConsent? ParentConsent { get; set; }

    // Asks a parent for permission for this profile (#392): the hash of a token made elsewhere,
    // usable for ParentConsent.KeepMonths from now.
    public ParentConsent AskParentConsent(string parentEmail, string tokenHash, DateTimeOffset now)
    {
        var consent = new ParentConsent
        {
            ProfileId = Id,
            ParentEmail = parentEmail,
            TokenHash = tokenHash,
            ExpiresAt = now.AddMonths(ParentConsent.KeepMonths)
        };
        ParentConsent = consent;

        return consent;
    }
}
