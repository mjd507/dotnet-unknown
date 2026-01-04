using DotNetUnknown.Lock;

namespace DotNetUnknown.Tests.Lock;

internal sealed class DistributedLockTests
{
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        TestProgram.SkipIfNoDockerAvailable();
    }

    [Test]
    public async Task BalanceTest()
    {
        // Given
        var distributedLockService = TestProgram.GetScopedService<DistributedLockService>();
        const int threadCount = 10;
        // Async When
        var task = (Func<Task>)(() =>
        {
            distributedLockService.Debit(10);
            return Task.CompletedTask;
        });
        var tasks = Enumerable.Range(0, threadCount).Select(_ => Task.Run(task)).ToArray();
        await Task.WhenAll(tasks);
        // Then
        Assert.That(distributedLockService.GetBalance(), Is.Zero);
    }
}