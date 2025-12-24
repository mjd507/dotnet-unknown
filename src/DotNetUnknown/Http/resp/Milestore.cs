using System.Text.Json.Serialization;

namespace DotNetUnknown.Http.resp;

public sealed record Milestone(
    [property: JsonPropertyName("number")] int Number,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("open_issues")]
    int OpenIssues,
    [property: JsonPropertyName("closed_issues")]
    int ClosedIssues,
    [property: JsonPropertyName("due_on")] DateTime DueOn
);