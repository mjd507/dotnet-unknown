using DotNetUnknown.Http.clients;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetUnknown.Tests.Http.clients;

internal sealed class GitHubHttpClientTests
{
    [Test]
    public async Task TestGitHubHttpClient()
    {
        // Given
        var serviceScope = StubHttpWebAppFactory.Instance.Services.CreateScope();
        var gitHubHttpClient = serviceScope.ServiceProvider.GetRequiredService<GitHubHttpClient>();
        // When
        var milestones = await gitHubHttpClient.GetMilestones("mjd507", "hello-world");
        var issues = await gitHubHttpClient.GetIssues("mjd507", "hello-world", "open");
        // Then
        using (Assert.EnterMultipleScope())
        {
            Assert.That(milestones![0].Number, Is.EqualTo(1));
            Assert.That(milestones[0].Title, Is.EqualTo("Release v1.0"));
            Assert.That(issues![0].Title, Is.EqualTo("Fix login bug"));
            Assert.That(issues[0].Assignee.login, Is.EqualTo("mjd507"));
        }
    }
}