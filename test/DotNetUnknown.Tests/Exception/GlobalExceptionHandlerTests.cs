using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetUnknown.Tests.Exception;

[TestFixture]
internal sealed class GlobalExceptionHandlerTests
{
    [OneTimeSetUp]
    public void Setup()
    {
        _exceptionTestWebAppFactory = new ExceptionTestWebAppFactory();
        _httpClient = _exceptionTestWebAppFactory.CreateClient();
    }

    [OneTimeTearDown]
    public void Teardown()
    {
        _exceptionTestWebAppFactory.Dispose();
        _httpClient.Dispose();
    }

    private class ExceptionTestWebAppFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddControllers()
                    .AddApplicationPart(Assembly.GetExecutingAssembly());
            });
        }
    }

    private WebApplicationFactory<Program> _exceptionTestWebAppFactory;
    private HttpClient _httpClient;

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
        var httpResponseMessage = await _httpClient.GetAsync(url);
        // Then
        Assert.That(httpResponseMessage.StatusCode, Is.EqualTo(statusCode));
        var problemDetails = await httpResponseMessage.Content.ReadFromJsonAsync<ProblemDetails>();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(problemDetails, Is.Not.Null);
            Assert.That(problemDetails.Instance, Is.EqualTo(url));
            Assert.That(problemDetails.Detail, Is.EqualTo(msg));
        }
    }
}