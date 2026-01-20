using Confluent.Kafka;
using DotNetUnknown.Support;

namespace DotNetUnknown.Kafka;

public class KafkaService(KafkaConfig kafkaConfig, ILogger<KafkaService> logger, ITestSupport testSupport)
{
    private const string Group = "test-consumer-group";
    private readonly TaskCompletionSource<bool> _consumerStartedTcs = new();
    private readonly KafkaConfig _kafkaConfig = kafkaConfig;

    public async Task Send(string message, string topic = KafkaConfig.Topic)
    {
        using var producer = _kafkaConfig.CreateProducer();
        var dr = await producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
        logger.LogInformation("Delivered '{MessageValue}' to '{DrTopicPartitionOffset}'", dr.Message.Value,
            dr.TopicPartitionOffset);
    }

    public async Task ListenerAsync(string topic = KafkaConfig.Topic, string groupId = Group,
        CancellationToken stoppingToken = default)
    {
        using var consumer = _kafkaConfig.CreateConsumer(groupId);

        consumer.Subscribe(topic);
        logger.LogInformation("Kafka consumer subscribed to topic: {Topic}", topic);
        try
        {
            _consumerStartedTcs.TrySetResult(true);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var cr = consumer.Consume(stoppingToken);
                    logger.LogInformation("Consumed message '{MessageValue}' at: '{CrTopicPartitionOffset}'.",
                        cr.Message.Value, cr.TopicPartitionOffset);
                    testSupport.Ack(cr.Message.Value);
                }
                catch (ConsumeException e)
                {
                    logger.LogError(e, "Kafka consume error: {ErrorReason}", e.Error.Reason);
                }
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Kafka consumer cancellation requested");
            consumer.Close();
        }
        finally
        {
            if (_consumerStartedTcs.Task.Status == TaskStatus.WaitingForActivation)
            {
                _consumerStartedTcs.TrySetException(new InvalidOperationException("Kafka consumer failed to start"));
            }
        }
    }

    public class KafkaBackgroundService(KafkaService kafkaService, ILogger<KafkaBackgroundService> logger)
        : BackgroundService
    {
        public Task ConsumerStartedTask => kafkaService._consumerStartedTcs.Task;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Kafka background service starting");
            kafkaService._kafkaConfig.CreateTopic();

            await kafkaService.ListenerAsync(stoppingToken: stoppingToken);
            logger.LogInformation("Kafka background service stopped");
        }
    }
}