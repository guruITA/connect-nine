using Connect.Domain.Users;

namespace Connect.Application.Profiles.GetCurrentProfile;

// ParentConsent is "pending" while a request to a parent exists and no permission was given yet
// (#395), and null otherwise, so the key stays out of the answer.
public sealed record CurrentProfileResult(
    string FirstName,
    AgeBand AgeBand,
    string Region,
    string? Bio,
    IReadOnlyList<TagResult> Tags,
    string? ParentConsent
);
