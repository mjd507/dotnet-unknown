using DotNetUnknown.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration) // appsettings.json
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithThreadId()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] [ThreadId {ThreadId}] {Message:lj}{NewLine}{Exception}")
);

builder.Services.AddTransient<LoggingUtils>();


var app = builder.Build();

// Optional: Adds middleware to log HTTP requests automatically via Serilog
app.UseSerilogRequestLogging();

app.Run();