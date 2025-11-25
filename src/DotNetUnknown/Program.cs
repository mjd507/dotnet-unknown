using DotNetUnknown.Exception;
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

builder.Services.AddControllers();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddTransient<LoggingUtils>();

builder.Services.AddSingleton<GlobalExceptionHandler>();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.MapControllers();
app.UseRouting();
app.UseExceptionHandler();

app.Run();