using System.Text;
using Asp.Versioning;
using DotNetUnknown.DbConfig;
using DotNetUnknown.Exception;
using DotNetUnknown.HealthCheck;
using DotNetUnknown.Http;
using DotNetUnknown.Logging;
using DotNetUnknown.Security;
using DotNetUnknown.Transaction;
using DotNetUnknown.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
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

#region JWT Authentication & Authorization

// for later DI
services.AddOptions<JwtSettings>().BindConfiguration("JwtSettings");
// for use now
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
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
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Key))
    };
});
services.AddAuthorization();
services.AddSingleton<JwtTokenUtils>();

#endregion

services.AddTransient<LoggingUtils>();

services.RegisterAppDbContext(builder.Configuration);

services.RegisterTransactionServices();

services.AddScoped<MxService>();

services.RegisterHttpClients();

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