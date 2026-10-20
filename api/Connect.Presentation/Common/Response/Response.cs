using System.Text.Json.Serialization;

namespace Connect.Presentation.Common.Response;

public record Response<TData, TMeta>(
    [property: JsonPropertyName("data")]
    TData Data,
    [property: JsonPropertyName("meta")]
    TMeta Meta);

public sealed record Response<TData>(TData Data) : Response<TData, DefaultMeta>(Data, new DefaultMeta());
