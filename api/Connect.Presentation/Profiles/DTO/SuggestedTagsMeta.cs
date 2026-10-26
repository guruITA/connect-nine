using System.Text.Json.Serialization;

namespace Connect.Presentation.Profiles.DTO;

public sealed record SuggestedTagsMeta(
    [property: JsonPropertyName("page")]
    int Page,
    [property: JsonPropertyName("pageSize")]
    int PageSize,
    [property: JsonPropertyName("total")]
    int Total);
