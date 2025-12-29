using DotNetUnknown.Resilience;

namespace DotNetUnknown.Tests.Resilience;

internal sealed class RetryServiceTests
{
    [Test]
    public async Task TestMethodNeedRetry()
    {
        var retryService = TestProgram.GetScopedService<RetryService>();
        var retryResult = await retryService.MethodNeedRetry();
        Assert.That(retryResult, Is.EqualTo("Retry Success!"));
        Assert.That(retryService.Count, Is.EqualTo(3));
    }
}