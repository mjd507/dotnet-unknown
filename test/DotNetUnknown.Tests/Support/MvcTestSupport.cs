using System.Net.Http.Headers;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetUnknown.Tests.Support;

public class MvcTestSupport
{
    private WebAppFactory _webAppFactory;
    protected HttpClient HttpClient;

    [OneTimeSetUp]
    public void SetUp()
    {
        _webAppFactory = new WebAppFactory();
        HttpClient = _webAppFactory.CreateClient();
        // add a api version before each request
        HttpClient.DefaultRequestHeaders.Add("X-Api-Version", "1.0");
        // add a jwt token before each request
        var jwtTokenUtils = GetRequiredService<JwtTokenTestSupport>();
        HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, jwtTokenUtils.NormalUserToken);
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _webAppFactory.Dispose();
        HttpClient.Dispose();
    }

    protected T GetRequiredService<T>() where T : notnull
    {
        return _webAppFactory.Services.GetRequiredService<T>();
    }
}

internal sealed class WebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddControllers()
                .AddApplicationPart(Assembly.GetExecutingAssembly());
            services.AddSingleton<JwtTokenTestSupport>();
        });
    }
}