using Connect.Domain.Users;

namespace Connect.Application.Profiles.GetPublicProfile;

// A value another member may not see, or that is empty, is null and never sent.
public sealed record GetPublicProfileResponse(
    string FirstName,
    AgeBand AgeBand,
    string? Region,
    string? Bio
);
