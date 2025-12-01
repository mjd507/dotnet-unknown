using System.Reflection;
using DotNetUnknown.Aop;
using DotNetUnknown.Tests.Support;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace DotNetUnknown.Tests.Aop;

[TestFixture]
internal sealed class MyTransactionalTests : BaseTestSupport
{
    private readonly Mock<MyTransactionSupport> _myTransactionSupportMock = new();

    protected override Action<IServiceCollection> ConfigureServicesAction()
    {
        return base.ConfigureServicesAction() + (services =>
        {
            services
                .AddScoped<MyTransactionSupport>(_ => _myTransactionSupportMock.Object)
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
        _myTransactionSupportMock.Verify(spy => spy.BeginTransaction(It.IsAny<MethodInfo>()), Times.Once());
        _myTransactionSupportMock.Verify(spy => spy.CommitTransaction(It.IsAny<MethodInfo>()), Times.Once());
    }
}

public interface IMyService
{
    [MyTransactional]
    public void MoneyMovement();
}

public class MyService : IMyService
{
    public void MoneyMovement()
    {
    }
}