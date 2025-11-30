using System.Net.Http.Headers;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetUnknown.Tests.Support;

public class MvcTestSupport : BaseTestSupport
{
    protected HttpClient HttpClient;

    protected override Action<IServiceCollection> ConfigureServicesAction()
    {
        return services =>
        {
            services.AddSingleton<JwtTokenTestSupport>()
                .AddControllers().AddApplicationPart(Assembly.GetExecutingAssembly());
        };
    }

    [OneTimeSetUp]
    public void MvcSetup()
    {
        HttpClient = WebAppFactory.CreateClient();
        // add a api version before each request
        HttpClient.DefaultRequestHeaders.Add("X-Api-Version", "1.0");
        // add a jwt token before each request
        var jwtTokenUtils = GetRequiredService<JwtTokenTestSupport>();
        HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, jwtTokenUtils.NormalUserToken);
    }

    [OneTimeTearDown]
    public void MvcTearDown()
    {
        HttpClient.Dispose();
    }
}