using System.Text.Json.Serialization;

namespace Connect.Presentation.Profiles.DTO;

// A value the server hides or that is empty is left out of the answer, never sent as null.
public sealed record PublicMemberProfileResponseData(
    string FirstName,
    string AgeBand,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? Region,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    string? Bio,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    IReadOnlyList<string>? Tags);
