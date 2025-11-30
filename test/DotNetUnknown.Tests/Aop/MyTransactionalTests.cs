using System.Reflection;
using DotNetUnknown.Aop;
using DotNetUnknown.Tests.Support;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace DotNetUnknown.Tests.Aop;

[TestFixture]
internal sealed class MyTransactionalTests : BaseTestSupport
{
    // Mock<ILogger> mockLogger = new() { CallBase = true };
    //
    private Mock<MyTransactionSupport> myTransactionSupportSpy = new() { CallBase = true };
    // {
    //     CallBase = true,
    // };

    protected override Action<IServiceCollection> ConfigureServicesAction()
    {
        return base.ConfigureServicesAction() + (services =>
        {
            services
                .AddScoped<MyTransactionSupport>()
                .AddProxiedScoped<IMyService, MyService, MyTransactionalInterceptor>();
        });
    }

    [Test]
    public void TestMyTransactionalAop()
    {
        // Given
        using var scope = WebAppFactory.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        var myService = serviceProvider.GetRequiredService<IMyService>();

        // When
        myService.MoneyMovement();

        // Then
        myTransactionSupportSpy.Verify(spy => spy.BeginTransaction(It.IsNotNull<MethodInfo>()), Times.Once());
        myTransactionSupportSpy.Verify(spy => spy.CommitTransaction(It.IsNotNull<MethodInfo>()), Times.Once());
    }
}

public interface IMyService
{
    [MyTransactional]
    public void MoneyMovement();
}

public class MyService(ILogger<MyService> logger) : IMyService
{
    public void MoneyMovement()
    {
        logger.LogInformation("processing money movement");
    }
}