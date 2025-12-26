using DotNetUnknown.DbConfig;
using DotNetUnknown.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DotNetUnknown.HealthCheck;

public static class HealthCheckExtension
{
    public static void AddHealthCheck(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<StartupBackgroundService>()
            .AddSingleton<StartupHealthCheck>();

        services.AddHealthChecks()
            .AddCheck<StartupHealthCheck>(name: "Startup", tags: ["ready"])
            .AddDbContextCheck<AppDbContext>(name: "Postgres", tags: ["db"]);

        if (configuration.GetValue<bool>(key: "HealthCheck:EnablePublisher"))
        {
            services.AddSingleton<IHealthCheckPublisher, HealthCheckPublisher>();

            services.Configure<HealthCheckPublisherOptions>(options =>
            {
                options.Delay = TimeSpan.FromSeconds(2);
                options.Period = TimeSpan.FromSeconds(60);
            });
        }
    }

    public static void UseHealthCheck(this WebApplication app)
    {
        app.MapHealthChecks(pattern: "/health", new HealthCheckOptions
        {
            ResponseWriter = (context, result) =>
            {
                context.Response.ContentType = "application/json";
                var response = new HealthCheckResponse(
                    Status: result.Status.ToString(),
                    Results: result.Entries.ToDictionary(
                        pair => pair.Key,
                        pair => new HealthCheckEntry(
                            Status: pair.Value.Status.ToString(),
                            Description: pair.Value.Description,
                            DurationMilliseconds: (long)pair.Value.Duration.TotalMilliseconds,
                            Exception: pair.Value.Exception?.Message,
                            // Safely convert heterogeneous objects to strings for JSON output
                            Data: pair.Value.Data.ToDictionary(kv => kv.Key, kv => kv.Value?.ToString())
                        )
                    )
                );

                return context.Response.WriteAsync(JsonUtils.Serialize(response));
            }
        });
    }
}