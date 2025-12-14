using System.Net.Http.Headers;
using DotNetUnknown.Aop;
using DotNetUnknown.DbConfig;
using DotNetUnknown.Tests.Aop;
using DotNetUnknown.Tests.Support;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Testcontainers.PostgreSql;

namespace DotNetUnknown.Tests;

[SetUpFixture]
internal sealed class TestProgram
{
    public static WebAppFactory WebAppFactory;
    public static HttpClient HttpClient;

    private readonly PostgreSqlContainer _pgContainer = new PostgreSqlBuilder()
        .WithImage("postgres:11")
        .WithDatabase("mydatabase")
        .WithUsername("myuser")
        .WithPassword("secret")
        .WithBindMount(Path.Combine(Directory.GetCurrentDirectory(), "../../../../../docker/db/init"),
            "/docker-entrypoint-initdb.d/")
        .Build();

    [OneTimeSetUp]
    public async Task GlobalSetup()
    {
        await TestContext.Out.WriteLineAsync("TestProgram Setup...");
        // postgres container
        await _pgContainer.StartAsync();
        // web application factory
        WebAppFactory = new WebAppFactory(_pgContainer.GetConnectionString());
        // http client for mvc test use
        HttpClient = WebAppFactory.CreateClient();
        ConfigureDefaultHttpClient();
    }

    [OneTimeTearDown]
    public async Task GlobalTeardown()
    {
        HttpClient.Dispose();
        await WebAppFactory.DisposeAsync();
        await _pgContainer.DisposeAsync();

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
}

public class WebAppFactory(string postgresConnectionString) : WebApplicationFactory<Program>
{
    public readonly Mock<MyTransactionSupport> MyTransactionSupportMock = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddSingleton<JwtTokenTestSupport>();
            // For AOP test
            services
                .AddScoped<MyTransactionSupport>(_ => MyTransactionSupportMock.Object)
                .AddProxiedScoped<IMyService, MyService, MyTransactionalInterceptor>();
        });

        builder.ConfigureTestServices(services =>
        {
            // Remove Application's DbContext
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<AppDbContext>();
            // Add postgres
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(postgresConnectionString));
        });
    }
}