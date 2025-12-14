using System.Reflection;
using DotNetUnknown.Aop;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace DotNetUnknown.Tests.Aop;

internal sealed class MyTransactionalTests
{
    [Test]
    public void TestMyTransactionalAop()
    {
        // Given
        var webAppFactory = TestProgram.WebAppFactory;
        using var scope = webAppFactory.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        var myService = serviceProvider.GetRequiredService<IMyService>();

        // When
        myService.MoneyMovement();

        // Then
        webAppFactory.MyTransactionSupportMock.Verify(spy => spy.BeginTransaction(It.IsAny<MethodInfo>()),
            Times.Once());
        webAppFactory.MyTransactionSupportMock.Verify(spy => spy.CommitTransaction(It.IsAny<MethodInfo>()),
            Times.Once());
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