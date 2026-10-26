using System.Text.Json.Serialization;

namespace Connect.Presentation.Profiles.DTO;

public sealed record MemberProfileResponseData(
    [property: JsonPropertyName("firstName")]
    string FirstName,
    [property: JsonPropertyName("ageBand")]
    string AgeBand,
    [property: JsonPropertyName("region")]
    string Region,
    [property: JsonPropertyName("bio")]
    string? Bio);
