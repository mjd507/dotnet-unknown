using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetUnknown.Tests.Support;

public abstract class BaseTestSupport
{
    protected WebAppFactory WebAppFactory;

    [OneTimeSetUp]
    public void BaseSetUp()
    {
        WebAppFactory = new WebAppFactory(ConfigureServicesAction(), ConfigureTestServicesAction());
    }


    protected virtual Action<IServiceCollection>? ConfigureServicesAction()
    {
        return null;
    }

    protected virtual Action<IServiceCollection>? ConfigureTestServicesAction()
    {
        return null;
    }

    [OneTimeTearDown]
    public void BaseTearDown()
    {
        WebAppFactory.Dispose();
    }

    protected T GetRequiredService<T>() where T : notnull
    {
        return WebAppFactory.Services.GetRequiredService<T>();
    }
}

public class WebAppFactory(
    Action<IServiceCollection>? configureServices,
    Action<IServiceCollection>? configureTestServices) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        if (configureServices != null)
        {
            builder.ConfigureServices(configureServices);
        }

        if (configureTestServices != null)
        {
            builder.ConfigureTestServices(configureTestServices);
        }
    }
}