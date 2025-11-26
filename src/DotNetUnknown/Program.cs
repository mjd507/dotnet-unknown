using Asp.Versioning;
using DotNetUnknown.Exception;
using DotNetUnknown.Logging;
using Serilog;
using Serilog.Enrichers.Span;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration) // appsettings.json
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithThreadId()
    .Enrich.WithSpan()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] [{ThreadId}] [{TraceId} - {SpanId}] {Message:lj}{NewLine}{Exception}")
);

builder.Services.AddControllers();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = false;
    options.ApiVersionReader = new HeaderApiVersionReader("X-Api-Version");
}).AddMvc();

builder.Services.AddTransient<LoggingUtils>();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();
app.UseRouting();
app.MapControllers();

app.Run();