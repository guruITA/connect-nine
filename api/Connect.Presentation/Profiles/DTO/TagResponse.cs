using System.Text.Json.Serialization;

namespace Connect.Presentation.Profiles.DTO;

public sealed record TagResponse(
    [property: JsonPropertyName("id")]
    Guid Id,
    [property: JsonPropertyName("name")]
    string Name);
