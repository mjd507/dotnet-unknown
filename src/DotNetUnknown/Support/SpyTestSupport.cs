namespace DotNetUnknown.Support;

public class SpyTestSupport(ILogger<SpyTestSupport> logger)
{
    public bool Ack(string message)
    {
        logger.LogInformation("Ack {message}", message);
        return true;
    }
}