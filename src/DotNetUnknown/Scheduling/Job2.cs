using DotNetUnknown.Scheduling.Executor;

namespace DotNetUnknown.Scheduling;

public class Job2(ILogger<Job2> logger) : IJob
{
    public async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(3000, stoppingToken);
        logger.LogInformation("Job 2 completed");
    }
}