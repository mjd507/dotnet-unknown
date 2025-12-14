using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;

namespace DotNetUnknown.Tests.Exception;

internal sealed class GlobalExceptionHandlerTests
{
    // Given
    internal static IEnumerable<object[]> ExceptionSourceProvider
    {
        get
        {
            yield return ["/business_exception", HttpStatusCode.Conflict, "this is a business exception"];
            yield return ["/system_exception", HttpStatusCode.InternalServerError, "this is a system exception"];
        }
    }

    [TestCaseSource(nameof(ExceptionSourceProvider))]
    public async Task TestGlobalExceptionHandler(string url, HttpStatusCode statusCode, string msg)
    {
        // When
        var httpResponseMessage = await TestProgram.HttpClient.GetAsync(url);
        // Then
        Assert.That(httpResponseMessage.StatusCode, Is.EqualTo(statusCode));
        var problemDetails = await httpResponseMessage.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.That(problemDetails, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(problemDetails.Instance, Is.EqualTo(url));
            Assert.That(problemDetails.Detail, Is.EqualTo(msg));
        }
    }
}