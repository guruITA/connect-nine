using Connect.Domain.Users;

namespace Connect.Application.Profiles.CreateProfile;

public sealed record CreateProfileCommand(
    Guid AccountId,
    string FirstName,
    AgeBand AgeBand,
    string Region,
    string? Bio
);
