using DotNetUnknown.Scheduling.Executor;

namespace DotNetUnknown.Scheduling;

public class Job1(ILogger<Job1> logger) : IJob
{
    public async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(1000, stoppingToken);
        logger.LogInformation("Job 1 completed");
    }
}