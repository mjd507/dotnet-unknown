using DotNetUnknown.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        // await kafka listener ready
        var kafkaListenerBackgroundService = TestProgram.WebAppFactory.Services.GetServices<IHostedService>()
            .OfType<KafkaService.KafkaBackgroundService>()
            .First();
        Assert.That(kafkaListenerBackgroundService, Is.Not.Null);
        var startedTask =
            await Task.WhenAny(kafkaListenerBackgroundService.ConsumerStartedTask, Task.Delay(TimeSpan.FromSeconds(5)));
        Assert.That(startedTask, Is.EqualTo(kafkaListenerBackgroundService.ConsumerStartedTask));
        Assert.That(startedTask.IsCompletedSuccessfully, Is.True);

        // Given
        const string msg = "hello, this is kafka integration test";
        var countdownEvent = new CountdownEvent(1);
        var testSupport = TestProgram.WebAppFactory.TestSupport;
        testSupport.Invocations.Clear();
        testSupport
            .Setup(spy => spy.Ack(msg))
            .Callback(() => countdownEvent.Signal());
        //  When
        await TestProgram.GetRequiredService<KafkaService>().Send(msg);
        countdownEvent.Wait(TimeSpan.FromSeconds(5));
        // Then
        testSupport.Verify(spy => spy.Ack(msg), Times.Once);
    }
}