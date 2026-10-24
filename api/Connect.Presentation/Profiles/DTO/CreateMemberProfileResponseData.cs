using System.Text.Json.Serialization;

namespace Connect.Presentation.Profiles.DTO;

public sealed record CreateMemberProfileResponseData(
    [property: JsonPropertyName("id")]
    Guid Id);
