using System.Text.Json.Serialization;

namespace Connect.Presentation.Profiles.DTO;

// parentConsent is left out, never sent as null, while no request is pending (#395).
public sealed record MemberProfileResponseData(
    string FirstName,
    string AgeBand,
    string Region,
    string? Bio,
    IReadOnlyList<TagResponse> Tags,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? ParentConsent);
