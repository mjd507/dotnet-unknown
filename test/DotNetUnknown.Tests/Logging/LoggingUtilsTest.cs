using DotNetUnknown.Logging;
using DotNetUnknown.Tests.Support;

namespace DotNetUnknown.Tests.Logging;

internal sealed class LoggingUtilsTest : MvcTestSupport
{
    [Test]
    public void TestLogInfo()
    {
        const string message = "Test execution complete for {TestName}";
        object?[] args = ["TestLogInfo"];
        var loggingUtils = GetRequiredService<LoggingUtils>();
        loggingUtils.LogInfo(message, args);
        Assert.Pass();
    }
}