using Polly;
using Polly.Retry;

namespace DotNetUnknown.Resilience;

public static class ResilienceExtension
{
    public static void RegisterResilience(this IServiceCollection services)
    {
        services.AddResiliencePipeline(key: "my-retry-pipeline", configure: pipeline =>
        {
            pipeline.AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(2),
                BackoffType = DelayBackoffType.Exponential,
                ShouldHandle = args => ValueTask.FromResult(
                    args.Outcome.Exception != null
                )
            });
        });

        services.AddScoped<RetryService>();
    }
}