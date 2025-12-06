using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetUnknown.Tests.Support;

public abstract class MvcTestSupport : BaseTestSupport
{
    protected HttpClient HttpClient;

    protected override Action<IServiceCollection> ConfigureServicesAction()
    {
        return services => { services.AddSingleton<JwtTokenTestSupport>(); };
    }

    [OneTimeSetUp]
    public void MvcSetup()
    {
        HttpClient = WebAppFactory.CreateClient();
        // add a api version before each request
        HttpClient.DefaultRequestHeaders.Add("X-Api-Version", "1.0");
        // add a jwt token before each request
        var jwtTokenUtils = GetRequiredService<JwtTokenTestSupport>();
        var normalUserToken = jwtTokenUtils.NormalUserToken;
        TestContext.Out.WriteLine($"{normalUserToken}");
        HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, normalUserToken);
    }

    [OneTimeTearDown]
    public void MvcTearDown()
    {
        HttpClient.Dispose();
    }
}