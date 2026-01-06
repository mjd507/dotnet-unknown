using DotNetUnknown.Scheduling.Executor;

namespace DotNetUnknown.Scheduling;

public class Job1(ILogger<Job1> logger) : IJob
{
    public Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task.Delay(2000, stoppingToken);
        logger.LogInformation("Job 1 completed");
        return Task.CompletedTask;
    }
}