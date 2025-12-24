using DotNetUnknown.Http.clients;
using DotNetUnknown.Http.Delegator;

namespace DotNetUnknown.Http;

public static class HttpServiceExtension
{
    public static void RegisterHttpClients(this IServiceCollection services)
    {
        // GitHub Client
        services
            .AddTransient<PerformanceAuditorDelegatingHandler>()
            .AddHttpClient<GitHubHttpClient>(configureClient =>
            {
                configureClient.BaseAddress = new Uri("https://api.github.com");
                configureClient.DefaultRequestHeaders.Accept.TryParseAdd("application/vnd.github.v3+json");
                configureClient.DefaultRequestHeaders.UserAgent.TryParseAdd("request");
            })
            .AddHttpMessageHandler<PerformanceAuditorDelegatingHandler>()
            .AddStandardResilienceHandler();

        // StackOverFlow Client
        services
            .AddHttpClient<StackoverflowHttpClient>(configureClient =>
            {
                configureClient.BaseAddress = new Uri("https://api.stackexchange.com");
                configureClient.DefaultRequestHeaders.UserAgent.TryParseAdd("request");
            })
            .AddStandardResilienceHandler();
    }
}