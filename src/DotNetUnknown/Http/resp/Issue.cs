using System.Text.Json.Serialization;

namespace DotNetUnknown.Http.resp;

public sealed record Issue(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("number")] int Number,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("assignee")]
    Assignee Assignee
);

public sealed record Assignee(
    [property: JsonPropertyName("login")] string login
);