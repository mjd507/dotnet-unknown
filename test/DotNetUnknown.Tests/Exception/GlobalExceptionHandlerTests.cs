using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DotNetUnknown.Tests.Exception;

[TestFixture]
internal sealed class GlobalExceptionHandlerTests
{
    [OneTimeSetUp]
    public void Setup()
    {
        _testWebApplicationFactory = new CustomTestAppFactory();
        _httpClient = _testWebApplicationFactory.CreateClient();
    }

    [OneTimeTearDown]
    public void Teardown()
    {
        _testWebApplicationFactory.Dispose();
        _httpClient.Dispose();
    }

    private CustomTestAppFactory _testWebApplicationFactory;
    private HttpClient _httpClient;

    private class CustomTestAppFactory : WebApplicationFactory<Program>
    {
        // public class TestOnlyController : ControllerBase
        // {
        //     [HttpGet("/business_exception")]
        //     public IActionResult BusinessException()
        //     {
        //         throw new BusinessException("this is a business exception");
        //     }
        // }

        // protected override void ConfigureWebHost(IWebHostBuilder builder)
        // {
        //     builder.ConfigureServices(services =>
        //     {
        //         services.AddControllers()
        //             .AddApplicationPart(Assembly.GetExecutingAssembly());
        //     });
        // }
    }

    [Test]
    public async Task TestGlobalExceptionHandler()
    {
        var httpResponseMessage = await _httpClient.GetAsync("/business_exception");

        Assert.That(httpResponseMessage.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));

        var problemDetails = await httpResponseMessage.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.That(problemDetails, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(problemDetails.Instance, Is.EqualTo("/business_exception"));
            Assert.That(problemDetails.Detail, Is.EqualTo("this is a business exception"));
        }
    }
}