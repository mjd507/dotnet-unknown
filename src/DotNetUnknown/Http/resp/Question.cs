using System.Net;
using System.Text.Json.Serialization;

namespace DotNetUnknown.Http.resp;

public sealed record Question(
    [property: JsonPropertyName("title")] string Title)
{
    public override string ToString()
    {
        return $"{WebUtility.HtmlDecode(Title)}";
    }
}

public sealed record QuestionContainer(
    [property: JsonPropertyName("items")] List<Question> Items);