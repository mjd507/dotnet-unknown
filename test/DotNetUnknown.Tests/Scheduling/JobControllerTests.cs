using System.Net;

namespace DotNetUnknown.Tests.Scheduling;

internal sealed class JobControllerTests
{
    [OneTimeSetUp]
    public void Setup()
    {
        TestProgram.SkipIfNoDockerAvailable();
    }

    [Test]
    public async Task TestJobTrigger()
    {
        var response = await TestProgram.HttpClient.GetAsync("/job/Job1");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }
}