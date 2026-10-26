using Connect.Domain.Common;

namespace Connect.Domain.ParentConsents;

// The request Connect sent to a parent for a profile of a young person under 18 (#392). Only
// its hash of the token is kept, never the token itself.
public sealed class ParentConsent : Entity
{
    // The client decided on 23 October 2026 that a parent's address is kept one month at most (#40).
    public const int KeepMonths = 1;

    public Guid ProfileId { get; set; }

    public required string ParentEmail { get; set; }

    public required string TokenHash { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }

    public DateTimeOffset? GivenAt { get; private set; }

    public bool IsUsable(DateTimeOffset now)
    {
        return GivenAt is null && !string.IsNullOrEmpty(ParentEmail) && now < ExpiresAt;
    }

    public void Give(DateTimeOffset now)
    {
        GivenAt ??= now;
        ParentEmail = string.Empty;
    }
}
