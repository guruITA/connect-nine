namespace Connect.Presentation.Profiles.DTO;

public sealed record MemberProfileResponseData(
    string FirstName,
    string AgeBand,
    string Region,
    string? Bio,
    IReadOnlyList<TagResponse> Tags);
