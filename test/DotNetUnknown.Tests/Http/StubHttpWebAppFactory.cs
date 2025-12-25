using DotNetUnknown.Http.clients;
using DotNetUnknown.Http.resp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetUnknown.Tests.Http;

[SetUpFixture]
internal sealed class StubHttpWebAppFactory
{
    public static WebApplicationFactory<Program> Instance;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        TestContext.Out.WriteLine("StubHttpWebAppFactory Setup...");
        Instance = TestProgram.WebAppFactory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services
                    .AddHttpClient<GitHubHttpClient>(client => { client.BaseAddress = new Uri("http://localhost"); })
                    .ConfigurePrimaryHttpMessageHandler(sp =>
                    {
                        var server = sp.GetRequiredService<IServer>() as TestServer;
                        return server!.CreateHandler();
                    });
                services
                    .AddHttpClient<StackoverflowHttpClient>(client =>
                    {
                        client.BaseAddress = new Uri("http://localhost");
                    })
                    .ConfigurePrimaryHttpMessageHandler(sp =>
                    {
                        var server = sp.GetRequiredService<IServer>() as TestServer;
                        return server!.CreateHandler();
                    });
            });

            builder.Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGet("/repos/{owner}/{repo}/milestones", () =>
                        Results.Json(new[]
                        {
                            new Milestone(1, "Release v1.0", 5, 10, new DateTime().AddDays(30))
                        })
                    );
                    endpoints.MapGet("/repos/{owner}/{repo}/issues", () =>
                        Results.Json(new[]
                        {
                            new Issue(123, 42, "Fix login bug", new Assignee("mjd507"))
                        })
                    );
                    endpoints.MapGet("/questions", () =>
                        Results.Json(
                            new QuestionContainer([
                                new Question("what's the difference between @Controller, @Service and @Repository?")
                            ])
                        )
                    );
                });
            });
        });
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Instance.Dispose();
        TestContext.Out.WriteLine("StubHttpWebAppFactory Teardown...");
    }
}