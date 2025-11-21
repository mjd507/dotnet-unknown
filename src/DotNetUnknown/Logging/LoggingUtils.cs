namespace DotNetUnknown.Logging;

public class LoggingUtils(ILogger<LoggingUtils> logger)
{
    public void LogInfo(string message, object?[] args)
    {
        logger.LogInformation(message, args);
    }
}