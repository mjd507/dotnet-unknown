using DotNetUnknown.Kafka;
using DotNetUnknown.Support;
using Moq;

namespace DotNetUnknown.Tests.Kafka;

internal sealed class KafkaServiceTest
{
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        TestProgram.SkipIfNoDockerAvailable();
    }


    [Test]
    public async Task TestProduceAndConsume()
    {
        // Given
        var msg = "hello, this is kafka integration test";
        var kafkaService = TestProgram.GetRequiredService<KafkaService>();
        var spyTestSupport = TestProgram.GetRequiredService<SpyTestSupport>();
        //  When
        await kafkaService.Send(msg);
        await Task.Delay(TimeSpan.FromSeconds(2));
        // Then
        Assert.That(spyTestSupport.Ack(It.IsAny<string>()), Is.True);
    }
}