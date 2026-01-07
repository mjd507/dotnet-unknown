namespace DotNetUnknown.Scheduling.Executor;

public interface IJob
{
    public Task ExecuteAsync(CancellationToken stoppingToken);
}