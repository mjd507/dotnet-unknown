using System.Net;
using System.Net.Http.Json;
using DotNetUnknown.Tests.Support;

namespace DotNetUnknown.Tests.ApiVersioning;

[TestFixture]
public class ApiVersioningTests : MvcTestSupport
{
    [SetUp]
    public void OneTimeSetUp()
    {
        HttpClient.DefaultRequestHeaders.Remove("X-Api-Version");
    }

    internal static IEnumerable<TestInfoRecord> RequestProvider
    {
        get
        {
            yield return new TestInfoRecord("1.0", "version 1.0 data", HttpStatusCode.OK);
            yield return new TestInfoRecord("1.1", "Unsupported API version", HttpStatusCode.BadRequest);
            yield return new TestInfoRecord(null, "Unspecified API version", HttpStatusCode.BadRequest);
        }
    }

    [TestCaseSource(nameof(RequestProvider))]
    public async Task TestApiVersioning(TestInfoRecord testInfoRecord)
    {
        // When
        HttpClient.DefaultRequestHeaders.Add("X-Api-Version", testInfoRecord.Version);
        var responseMessage = await HttpClient.GetAsync("/ApiVersioning/v-1-0");
        // Then
        var statusCode = responseMessage.StatusCode;
        var responseJson = await responseMessage.Content.ReadFromJsonAsync<ResponseRecord>();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(statusCode, Is.EqualTo(testInfoRecord.Status));
            Assert.That(responseJson?.Title, Is.EqualTo(testInfoRecord.Title));
        }
    }

    public sealed record TestInfoRecord(string? Version, string Title, HttpStatusCode Status);

    private record ResponseRecord(string? Type, string Title, HttpStatusCode? Status, string? Detail, string? TraceId);
}