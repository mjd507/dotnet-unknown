using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DotNetUnknown.HealthCheck;

public sealed class StartupHealthCheck : IHealthCheck
{
    private volatile bool _isReady;

    public bool StartupCompleted
    {
        get => _isReady;
        set => _isReady = value;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (StartupCompleted)
        {
            return Task.FromResult(HealthCheckResult.Healthy(description: "The startup task has completed."));
        }

        return Task.FromResult(HealthCheckResult.Unhealthy(description: "That startup task is still running."));
    }
}

public class StartupBackgroundService(StartupHealthCheck startupHealthCheck) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Simulate the effect of a long-running task.
        await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);

        startupHealthCheck.StartupCompleted = true;
    }
}