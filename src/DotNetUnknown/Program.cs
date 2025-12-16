using System.Text;
using Asp.Versioning;
using DotNetUnknown.DbConfig;
using DotNetUnknown.Exception;
using DotNetUnknown.Logging;
using DotNetUnknown.Security;
using DotNetUnknown.Transaction;
using DotNetUnknown.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
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

var services = builder.Services;
services.AddControllers(options => { options.Filters.Add(new AuthorizeFilter()); });
services.AddExceptionHandler<GlobalExceptionHandler>();
services.AddProblemDetails();

services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = false;
    options.ApiVersionReader = new HeaderApiVersionReader("X-Api-Version");
}).AddMvc();

#region JWT Authentication & Authorization

var jwtSettings = builder.Configuration.GetRequiredSection("JwtSettings");
var jwtKey = Encoding.ASCII.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException());
services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Set to true in production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(jwtKey)
    };
});
services.AddAuthorization();
services.AddSingleton<JwtTokenUtils>();

#endregion

services.AddTransient<LoggingUtils>();

services.RegisterAppDbContext();

services.RegisterTransactionServices();

services.AddScoped<MxService>();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.EnsureDatabaseCreated();

app.Run();