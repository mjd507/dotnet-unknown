using System.Collections.Concurrent;
using DotNetUnknown.Resilience;

namespace DotNetUnknown.Tests.Resilience;

internal sealed class RateLimitServiceTests
{
    [Test]
    public async Task TestRateLimitMethod()
    {
        // GIVEN
        const int threadCount = 20;
        var threadNameList = new ConcurrentBag<string>();

        var rateLimitService = TestProgram.GetScopedService<RateLimitService>();

        var task = (Func<Task>)(() =>
        {
            rateLimitService.RateLimitMethod();
            threadNameList.Add(Environment.CurrentManagedThreadId.ToString());
            return Task.CompletedTask;
        });

        var tasks = Enumerable.Range(0, threadCount).Select(_ => Task.Run(task)).ToArray();
        // WHEN
        var timer = PrintLogTimer(threadNameList);
        await Task.WhenAll(tasks);
        // Then
        Assert.That(threadNameList, Has.Count.EqualTo(threadCount));
        timer.Change(dueTime: Timeout.Infinite, period: Timeout.Infinite);
    }

    private static Timer PrintLogTimer(ConcurrentBag<string> threadNameList)
    {
        var timer = new Timer(_ =>
                TestContext.Out.WriteLine($"rate limit test: running threads size: {threadNameList.Count}"),
            state: null, dueTime: 0, period: 1100);
        return timer;
    }
}