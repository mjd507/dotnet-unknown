using DotNetUnknown.Http.clients;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetUnknown.Tests.Http.clients;

internal sealed class StackoverflowHttpClientTests
{
    [Test]
    public async Task TestStackoverflowClient()
    {
        // Given
        var serviceScope = StubHttpWebAppFactory.Instance.Services.CreateScope();
        var stackoverflowClient = serviceScope.ServiceProvider.GetRequiredService<StackoverflowHttpClient>();
        // When
        var questionsContainer = await stackoverflowClient.GetQuestions("spring", "votes");
        // Then
        using (Assert.EnterMultipleScope())
        {
            Assert.That(questionsContainer!.Items[0].Title,
                Is.EqualTo("what's the difference between @Controller, @Service and @Repository?"));
        }
    }
}