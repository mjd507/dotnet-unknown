using DotNetUnknown.Caching;
using Moq;

namespace DotNetUnknown.Tests.Caching;

internal sealed class RedisCacheServiceTest
{
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        TestProgram.SkipIfNoDockerAvailable();
    }

    [Test]
    public void GetUser()
    {
        // Given
        var redisCacheService = TestProgram.GetScopedService<RedisCacheService>();
        var testSupport = TestProgram.WebAppFactory.TestSupport;
        testSupport.Invocations.Clear();
        // When
        var userData = redisCacheService.GetUser(1);
        // Then
        Assert.That(userData, Is.Not.Null);
        Assert.That(userData.Id, Is.EqualTo(1));
        Assert.That(userData.Name, Is.EqualTo("JD"));
        Assert.That(userData.Age, Is.EqualTo(25));
        testSupport.Verify(spy => spy.Ack(It.IsAny<string>()), Times.Once);
        testSupport.Invocations.Clear();
        // When 
        var userFromCache = redisCacheService.GetUser(1);
        // Then (from cache)
        testSupport.Verify(spy => spy.Ack(It.IsAny<string>()), Times.Never);
        Assert.That(userFromCache, Is.Not.Null);
        Assert.That(userFromCache, Is.EqualTo(userData));
        testSupport.Invocations.Clear();
    }
}