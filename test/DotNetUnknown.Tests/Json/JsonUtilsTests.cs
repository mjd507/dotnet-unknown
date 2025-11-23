using System.Text.Json.Serialization;
using DotNetUnknown.Json;

namespace DotNetUnknown.Tests.Json;

[TestFixture]
internal sealed class JsonUtilsTests
{
    private class WeatherForecast
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Summary;

        [JsonPropertyOrder(-1)] public DateTimeOffset Date { get; init; }
        public int TemperatureCelsius { get; init; }

        [JsonPropertyName("cloudCover")]
        [JsonPropertyOrder(1)]
        public CloudCover? Sky { get; set; }

        internal enum CloudCover
        {
            Clear,

            [JsonStringEnumMemberName("Partial Cloudy")]
            Partial,
            Overcast
        }
    }

    [Test]
    public void TestSerialization()
    {
        // Given
        var weatherForecast = new WeatherForecast
        {
            Date = new DateTimeOffset(new DateTime(2025, 01, 01)),
            TemperatureCelsius = 123,
            Sky = WeatherForecast.CloudCover.Partial,
            Summary = null
        };
        // [Serialize] When
        var json = JsonUtils.Serialize(weatherForecast);
        const string expectedJson =
            """
            {"date":"2025-01-01T00:00:00+08:00","temperatureCelsius":123,"cloudCover":"Partial Cloudy"}
            """;
        // [Serialize] Then
        Assert.That(json, Is.EqualTo(expectedJson));

        // [Deserialize] When
        var expectedObj = JsonUtils.Deserialize<WeatherForecast>(json);
        using (Assert.EnterMultipleScope())
        {
            // [Deserialize] Then
            Assert.That(weatherForecast.Date, Is.EqualTo(expectedObj!.Date));
            Assert.That(weatherForecast.TemperatureCelsius, Is.EqualTo(expectedObj.TemperatureCelsius));
            Assert.That(weatherForecast.Summary, Is.EqualTo(expectedObj.Summary));
            Assert.That(weatherForecast.Sky, Is.EqualTo(expectedObj.Sky));
        }
    }
}