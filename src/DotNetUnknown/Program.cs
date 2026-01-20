using Asp.Versioning;
using DotNetUnknown.DbConfig;
using DotNetUnknown.Exception;
using DotNetUnknown.HealthCheck;
using DotNetUnknown.Http;
using DotNetUnknown.Kafka;
using DotNetUnknown.Lock;
using DotNetUnknown.Logging;
using DotNetUnknown.Resilience;
using DotNetUnknown.Scheduling;
using DotNetUnknown.Scheduling.Executor;
using DotNetUnknown.Security;
using DotNetUnknown.Support;
using DotNetUnknown.Transaction;
using DotNetUnknown.Validation;
using Microsoft.AspNetCore.Mvc.Authorization;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
);

var services = builder.Services;
services.AddControllers(options => { options.Filters.Add(new AuthorizeFilter()); });

services.AddExceptionHandler<GlobalExceptionHandler>();
services.AddProblemDetails();

services.AddHealthCheck(builder.Configuration);

services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = false;
    options.ApiVersionReader = new HeaderApiVersionReader("X-Api-Version");
}).AddMvc();

services.RegisterJwtAuth(builder.Configuration);

services.RegisterAppDbContext(builder.Configuration);

services.RegisterTransactionServices();

services.RegisterHttpClients();

services.RegisterResilience();

services.RegisterLock(builder.Configuration);

services.AddJobServices();
services.AddJobExecutors(builder.Configuration);

services.AddTransient<LoggingUtils>();

services.AddScoped<MxService>();

services.AddKafka();

services.AddSingleton<ITestSupport, TestSupport>();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseHealthCheck();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.EnsureDatabaseCreated();

app.Run();