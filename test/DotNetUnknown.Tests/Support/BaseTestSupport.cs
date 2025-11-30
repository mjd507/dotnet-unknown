using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetUnknown.Tests.Support;

public class BaseTestSupport
{
    protected WebAppFactory WebAppFactory;

    [OneTimeSetUp]
    public void BaseSetUp()
    {
        WebAppFactory = new WebAppFactory(ConfigureServicesAction());
    }

    protected virtual Action<IServiceCollection>? ConfigureServicesAction()
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

public class WebAppFactory(Action<IServiceCollection>? configureServices) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        if (configureServices != null)
        {
            builder.ConfigureServices(configureServices);
        }
    }
}