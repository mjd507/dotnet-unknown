using DotNetUnknown.Scheduling.Executor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DotNetUnknown.Scheduling;

[ApiController]
[Route("job")]
public class JobController(IServiceProvider serviceProvider) : ControllerBase
{
    [HttpGet]
    [Route("{job}")]
    public async Task<IResult> Trigger(string job)
    {
        var jobExecutor = serviceProvider.GetKeyedService<JobExecutor>(job);
        if (jobExecutor == null || !serviceProvider
                .GetRequiredService<IOptionsMonitor<Dictionary<string, CrontabOptions>>>().CurrentValue
                .TryGetValue(job, out var options)) return Results.NotFound();

        serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(job).LogInformation("开始执行Job");

        return await jobExecutor.ExecuteOnceAsync(options, serviceProvider, new CancellationToken(false)) switch
        {
            true => Results.NoContent(),
            false => Results.StatusCode(500),
            _ => Results.Conflict()
        };
    }
}