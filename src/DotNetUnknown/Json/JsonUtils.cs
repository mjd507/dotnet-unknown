using System.Text.Json;
using System.Text.Json.Serialization;

namespace DotNetUnknown.Json;

public static class JsonUtils
{
    private static readonly JsonSerializerOptions Opts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) // enum as strings
        },
        IncludeFields = true,
    };

    public static string Serialize<T>(T obj)
    {
        return JsonSerializer.Serialize<T>(obj, Opts);
    }

    public static T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, Opts);
    }
}