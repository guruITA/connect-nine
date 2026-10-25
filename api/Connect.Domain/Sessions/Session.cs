using Connect.Domain.Common;

namespace Connect.Domain.Sessions;

public sealed class Session : Entity
{

    public required Guid AccountId { get; init; }

    public required string TokenHash { get; init; }

    public required DateTimeOffset ExpiresAt { get; init; }

    public DateTimeOffset? RevokedAt { get; private set; }

    public bool IsActive(DateTimeOffset now)
        => RevokedAt is null && now < ExpiresAt;

    public void Revoke(DateTimeOffset now)
        => RevokedAt ??= now;
}
