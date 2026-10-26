namespace Connect.Presentation.Profiles.DTO;

public sealed record CreateMemberProfileRequest(
    string? FirstName,
    string? AgeBand,
    string? Region,
    string? Bio
);
