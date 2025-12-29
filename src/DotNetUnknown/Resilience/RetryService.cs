using Polly.Registry;

namespace DotNetUnknown.Resilience;

public class RetryService(ResiliencePipelineProvider<string> pipelineProvider)
{
    public int Count = 0;

    public async Task<string> MethodNeedRetry()
    {
        var retryPipeline = pipelineProvider.GetPipeline(key: "my-retry-pipeline");

        return await retryPipeline.ExecuteAsync(_ =>
        {
            if (Interlocked.Increment(ref Count) < 3)
            {
                throw new System.Exception($"Simulated failure, {Count}");
            }

            return ValueTask.FromResult("Retry Success!");
        });
    }
}