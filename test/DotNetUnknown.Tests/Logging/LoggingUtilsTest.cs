using DotNetUnknown.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace DotNetUnknown.Tests.Logging;

internal sealed class LoggingUtilsTest
{
    private LoggingUtils _loggerFromSeriLog;
    private LoggingUtils _loggingUtilsFromSimpleConsole;

    [OneTimeSetUp]
    public void SetUpLoggingUtils()
    {
        // create a minimal DI container using [Microsoft.Extensions.Logging.Console] for testing.
        _loggingUtilsFromSimpleConsole = SimpleConsoleLog();

        // create a minimal DI container using [SeriLog] for testing.
        _loggerFromSeriLog = Serilog();

        return;

        LoggingUtils SimpleConsoleLog()
        {
            ServiceCollection serviceCollection = [];
            serviceCollection.AddLogging(builder => builder.AddSimpleConsole(options =>
            {
                options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss.fff] ";
                options.SingleLine = true;
            }));
            serviceCollection.AddTransient<LoggingUtils>();
            using var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider.GetRequiredService<LoggingUtils>();
        }

        LoggingUtils Serilog()
        {
            ServiceCollection serviceCollection = [];

            var logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                .WriteTo.Console(
                    outputTemplate:
                    "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] [ThreadId {ThreadId}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            serviceCollection.AddLogging(builder => builder.AddSerilog(logger));

            serviceCollection.AddTransient<LoggingUtils>();

            using var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider.GetRequiredService<LoggingUtils>();
        }
    }

    [Test]
    public void TestLogInfo()
    {
        const string message = "Test execution complete for {TestName}";
        object?[] args = ["TestLogInfo"];
        _loggingUtilsFromSimpleConsole.LogInfo(message, args);
        _loggerFromSeriLog.LogInfo(message, args);
        Assert.Pass();
    }
}