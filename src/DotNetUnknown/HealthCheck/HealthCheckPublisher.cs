using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DotNetUnknown.HealthCheck;

public sealed class HealthCheckPublisher(ILogger<HealthCheckPublisher> logger, IWebHostEnvironment environment)
    : IHealthCheckPublisher
{
    public Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
    {
        var logLevel = report.Status == HealthStatus.Healthy ? LogLevel.Information : LogLevel.Error;

        logger.Log(
            logLevel,
            message: "{HealthStatus} {AppName} {Timestamp} {EnvironmentName} Readiness Probe Status: {@Report}",
            report.Status,
            environment.ApplicationName,
            DateTime.Now,
            environment.EnvironmentName,
            report);

        return Task.CompletedTask;
    }
}