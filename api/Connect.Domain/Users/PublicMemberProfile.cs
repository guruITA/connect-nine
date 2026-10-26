namespace Connect.Domain.Users;

// The part of a profile another member may read: no id, no account, no dates.
public sealed record PublicMemberProfile(
    string FirstName,
    AgeBand AgeBand,
    string Region,
    string? Bio
);
