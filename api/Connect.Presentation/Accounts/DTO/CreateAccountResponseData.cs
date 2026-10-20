using System.Text.Json.Serialization;

namespace Connect.Presentation.Accounts.DTO;

public sealed record CreateAccountResponseData(
    [property: JsonPropertyName("id")]
    Guid Id,
    [property: JsonPropertyName("email")]
    string Email);
