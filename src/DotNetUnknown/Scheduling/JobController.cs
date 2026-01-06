using Microsoft.AspNetCore.Mvc;

namespace DotNetUnknown.Scheduling;

[ApiController]
[Route("job")]
public class JobController : ControllerBase
{
    [HttpGet]
    [Route("{job}")]
    public IActionResult RequiredVersionOne(string job)
    {
        return Ok(job);
    }

    // app.MapPost("/job/{job}", static async (HttpContext context, string job) =>
    // {
    //     var jobExecutor = context.RequestServices.GetKeyedService<JobExecutor>(job);
    //     if (jobExecutor == null || !context.RequestServices
    //             .GetRequiredService<IOptionsMonitor<Dictionary<string, CrontabOptions>>>().CurrentValue
    //             .TryGetValue(job, out var options)) return Results.NotFound();
    //
    //     context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(job).LogInformation("开始执行Job");
    //
    //     return await jobExecutor.ExecuteOnceAsync(options, context.RequestServices, context.RequestAborted) switch
    //     {
    //         true => Results.NoContent(),
    //         false => Results.StatusCode(500),
    //         _ => Results.Conflict()
    //     };
    // });
}