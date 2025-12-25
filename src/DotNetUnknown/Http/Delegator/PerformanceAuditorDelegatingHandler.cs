namespace DotNetUnknown.Http.Delegator;

public class PerformanceAuditorDelegatingHandler(ILogger<PerformanceAuditorDelegatingHandler> logger)
    : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Start {RequestRequestUri} at {DateTime}", request.RequestUri, DateTime.Now);

        // original call
        var response = await base.SendAsync(request, cancellationToken);

        logger.LogInformation(response.ToString());
        logger.LogInformation("Finish {RequestRequestUri} at {DateTime}", request.RequestUri, DateTime.Now);

        return response;
    }
}