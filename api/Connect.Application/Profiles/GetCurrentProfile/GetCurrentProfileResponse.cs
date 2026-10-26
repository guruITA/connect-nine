using Connect.Domain.Users;

namespace Connect.Application.Profiles.GetCurrentProfile;

public sealed record CurrentProfileResult(
    string FirstName,
    AgeBand AgeBand,
    string Region,
    string? Bio,
    IReadOnlyList<TagResult> Tags
);
