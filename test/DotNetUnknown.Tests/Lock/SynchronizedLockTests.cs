using DotNetUnknown.Lock;

namespace DotNetUnknown.Tests.Lock;

internal sealed class SynchronizedLockServiceTests
{
    [Test]
    public async Task BalanceTest()
    {
        // Given
        var lockService = TestProgram.GetScopedService<SynchronizedLockService>();
        const int threadCount = 10;
        // Async When
        var task = (Func<Task>)(() =>
        {
            lockService.Debit(10);
            return Task.CompletedTask;
        });
        var tasks = Enumerable.Range(0, threadCount).Select(_ => Task.Run(task)).ToArray();
        await Task.WhenAll(tasks);
        // Then
        Assert.That(lockService.GetBalance(), Is.EqualTo(0));
    }
}