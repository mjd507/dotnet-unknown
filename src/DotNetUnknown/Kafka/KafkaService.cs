using Confluent.Kafka;
using DotNetUnknown.Support;

namespace DotNetUnknown.Kafka;

public class KafkaService(KafkaConfig kafkaConfig, ILogger<KafkaService> logger, SpyTestSupport spyTestSupport)
{
    private const string Group = "test-consumer-group";
    private readonly KafkaConfig _kafkaConfig = kafkaConfig;

    public async Task Send(string message, string topic = KafkaConfig.Topic)
    {
        using var producer = _kafkaConfig.CreateProducer();
        var dr = await producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
        logger.LogInformation("Delivered '{MessageValue}' to '{DrTopicPartitionOffset}'", dr.Message.Value,
            dr.TopicPartitionOffset);
    }

    public void Listener(string topic = KafkaConfig.Topic, string groupId = Group)
    {
        using var consumer = _kafkaConfig.CreateConsumer(groupId);

        consumer.Subscribe(topic);

        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) =>
        {
            // Prevent the process from terminating.
            e.Cancel = true;
            cts.Cancel();
        };

        try
        {
            while (true)
            {
                try
                {
                    var cr = consumer.Consume(cts.Token);
                    logger.LogInformation("Consumed message '{MessageValue}' at: '{CrTopicPartitionOffset}'.",
                        cr.Message.Value, cr.TopicPartitionOffset);
                    spyTestSupport.Ack(cr.Message.Value);
                }
                catch (ConsumeException e)
                {
                    logger.LogInformation("Error occured: {ErrorReason}", e.Error.Reason);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Ensure the consumer leaves the group cleanly and final offsets are committed.
            consumer.Close();
        }
    }

    public class KafkaBackgroundService(KafkaService kafkaService) : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            kafkaService._kafkaConfig.CreateTopic();
            kafkaService.Listener();
            return Task.CompletedTask;
        }
    }
}