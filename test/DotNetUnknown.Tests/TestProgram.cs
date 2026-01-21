using System.Net.Http.Headers;
using DotNet.Testcontainers.Builders;
using DotNetUnknown.Aop;
using DotNetUnknown.DbConfig;
using DotNetUnknown.Kafka;
using DotNetUnknown.Support;
using DotNetUnknown.Tests.Aop;
using DotNetUnknown.Tests.Support;
using Medallion.Threading;
using Medallion.Threading.Postgres;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Moq;
using Testcontainers.Kafka;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace DotNetUnknown.Tests;

[SetUpFixture]
internal sealed class TestProgram
{
    public static WebAppFactory WebAppFactory;
    public static HttpClient HttpClient;

    private static bool _isDockerAvailable;

    private static async Task<bool> CheckDockerAvailability()
    {
        try
        {
            await new ContainerBuilder("postgres:11").Build().StartAsync();
            await new ContainerBuilder("postgres:11").Build().DisposeAsync().ConfigureAwait(false);
            return true;
        }
        catch (System.Exception ex)
        {
            await TestContext.Progress.WriteLineAsync($"Docker unavailable: {ex.Message}");
            return false;
        }
    }

    public static void SkipIfNoDockerAvailable()
    {
        if (!_isDockerAvailable)
        {
            Assert.Inconclusive("Docker unavailable. Skipping tests.");
        }
    }

    [OneTimeSetUp]
    public async Task GlobalSetup()
    {
        await TestContext.Out.WriteLineAsync("TestProgram Setup...");
        _isDockerAvailable = await CheckDockerAvailability();
        var postgresConnectionString = string.Empty;
        var kafkaConnectionString = string.Empty;
        var redisConnectionString = string.Empty;
        if (_isDockerAvailable)
        {
            var pgContainer = new PostgreSqlBuilder("postgres:11")
                .WithDatabase("mydatabase")
                .WithUsername("myuser")
                .WithPassword("secret")
                .WithBindMount(Path.Combine(Directory.GetCurrentDirectory(), "../../../../../docker/db/init"),
                    "/docker-entrypoint-initdb.d/")
                .Build();

            // start postgres 
            await pgContainer.StartAsync();
            postgresConnectionString = pgContainer.GetConnectionString();
            // kafka
            var kafkaContainer = new KafkaBuilder("apache/kafka:4.1.1")
                .WithKRaft()
                .Build();
            await kafkaContainer.StartAsync();
            kafkaConnectionString = kafkaContainer.GetBootstrapAddress();

            // redis
            var redisContainer = new RedisBuilder("redis:8.4.0")
                .Build();
            await redisContainer.StartAsync();
            redisConnectionString = redisContainer.GetConnectionString();
        }

        var connectionString = new ConnectionString(
            postgresConnectionString,
            kafkaConnectionString,
            redisConnectionString
        );

        await TestContext.Out.WriteLineAsync("connectionStrings :" + connectionString);

        // web application factory
        WebAppFactory = new WebAppFactory(connectionString, _isDockerAvailable);
        // http client for mvc test use
        HttpClient = WebAppFactory.CreateClient();
        ConfigureDefaultHttpClient();
    }

    [OneTimeTearDown]
    public async Task GlobalTeardown()
    {
        HttpClient.Dispose();
        await WebAppFactory.DisposeAsync();

        await TestContext.Out.WriteLineAsync("TestProgram Teared Down...");
    }

    private static void ConfigureDefaultHttpClient()
    {
        // add a api version 
        SetDefaultApiVersion();
        // add a jwt token 
        SetDefaultJwtToken();
    }

    public static void SetDefaultJwtToken()
    {
        var jwtTokenUtils = GetRequiredService<JwtTokenTestSupport>();
        var normalUserToken = jwtTokenUtils.NormalUserToken;
        TestContext.Out.WriteLine($"{normalUserToken}");
        HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, normalUserToken);
    }

    public static void SetDefaultApiVersion()
    {
        HttpClient.DefaultRequestHeaders.Add("X-Api-Version", "1.0");
    }

    public static T GetRequiredService<T>() where T : notnull
    {
        return WebAppFactory.Services.GetRequiredService<T>();
    }

    public static T GetScopedService<T>() where T : notnull
    {
        var serviceScope = WebAppFactory.Services.CreateScope();
        return serviceScope.ServiceProvider.GetService<T>()!;
    }
}

public record ConnectionString(string Postgres, string Kafka, string Redis);

public class WebAppFactory(ConnectionString connectionString, bool isDockerAvailable) : WebApplicationFactory<Program>
{
    public readonly Mock<MyTransactionSupport> MyTransactionSupportMock = new();
    public readonly Mock<ITestSupport> TestSupport = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddSingleton<JwtTokenTestSupport>();
            // For AOP test
            services
                .AddScoped<MyTransactionSupport>(_ => MyTransactionSupportMock.Object)
                .AddProxiedScoped<IMyService, MyService, MyTransactionalInterceptor>();
            // for Mock Test Support
            services
                .RemoveAll<ITestSupport>()
                .AddSingleton<ITestSupport>(_ => TestSupport.Object);
        });

        builder.ConfigureTestServices(services =>
        {
            // Remove Application's DbContext
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<AppDbContext>();
            // Add postgres
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString.Postgres));
            // lock postgres setup
            services.RemoveAll<IDistributedLockProvider>();
            services
                .AddSingleton<IDistributedLockProvider>(_ =>
                    new PostgresDistributedSynchronizationProvider(connectionString.Postgres));
            // kafka
            services.RemoveAll<KafkaOptions>();
            services.Configure<KafkaOptions>(options => { options.BootstrapServers = connectionString.Kafka; });
            if (!isDockerAvailable)
            {
                // remove kafka background service, prevent connection err
                var toRemove = services
                    .FirstOrDefault(d => d.ServiceType == typeof(IHostedService)
                                         && d.ImplementationType == typeof(KafkaService.KafkaBackgroundService))!;
                services.Remove(toRemove);
            }

            // redis
            services.RemoveAll<IDistributedCache>();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = connectionString.Redis;
                options.InstanceName = "DotNetUnknown:";
            });
        });
    }
}