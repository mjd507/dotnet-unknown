namespace DotNetUnknown.Support;

public interface ITestSupport
{
    public bool Ack(string message);
}

public class TestSupport(ILogger<ITestSupport> logger) : ITestSupport
{
    public bool Ack(string message)
    {
        logger.LogInformation("Ack {message}", message);
        return true;
    }
}