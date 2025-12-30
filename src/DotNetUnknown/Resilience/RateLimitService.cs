using Polly.Registry;

namespace DotNetUnknown.Resilience;

public sealed class RateLimitService(ResiliencePipelineProvider<string> pipelineProvider)
{
    public void RateLimitMethod()
    {
        var rateLimitPipeline = pipelineProvider.GetPipeline(key: "my-rate-limit-pipeline");

        rateLimitPipeline.Execute(_ => Thread.Sleep(millisecondsTimeout: 1000));
    }
}