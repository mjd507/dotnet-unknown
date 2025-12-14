using DotNetUnknown.Logging;

namespace DotNetUnknown.Tests.Logging;

internal sealed class LoggingUtilsTest
{
    [Test]
    public void TestLogInfo()
    {
        const string message = "Test execution complete for {TestName}";
        object?[] args = ["TestLogInfo"];
        var loggingUtils = TestProgram.GetRequiredService<LoggingUtils>();
        loggingUtils.LogInfo(message, args);
        Assert.Pass();
    }
}