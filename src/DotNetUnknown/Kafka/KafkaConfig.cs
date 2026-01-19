using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace DotNetUnknown.Kafka;

public class KafkaConfig
{
    public const string Topic = "test-topic";
    private const string BootstrapServers = "localhost:9092";

    private static ClientConfig CommonConfig()
    {
        var config = new ClientConfig
        {
            BootstrapServers = BootstrapServers,
        };
        return config;
    }

    public IProducer<Null, string> CreateProducer()
    {
        var config = CommonConfig();

        // If serializers are not specified, default serializers from
        // `Confluent.Kafka.Serializers` will be automatically used where
        // available. Note: by default strings are encoded as UTF8.
        return new ProducerBuilder<Null, string>(config)
            .Build();
    }

    public IConsumer<Ignore, string> CreateConsumer(string groupId)
    {
        var commonConfig = CommonConfig();
        var config = new ConsumerConfig
        {
            GroupId = groupId,
            BootstrapServers = commonConfig.BootstrapServers,
            // Note: The AutoOffsetReset property determines the start offset in the event
            // there are not yet any committed offsets for the consumer group for the
            // topic/partitions of interest. By default, offsets are committed
            // automatically, so in this example, consumption will only start from the
            // earliest message in the topic 'my-topic' the first time you run the program.
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        return new ConsumerBuilder<Ignore, string>(config)
            .Build();
    }

    public void CreateTopic(string topic = Topic)
    {
        var clientConfig = CommonConfig();
        var adminClient = new AdminClientBuilder(clientConfig).Build();
        adminClient.DeleteTopicsAsync([topic]).Wait();
        adminClient.CreateTopicsAsync([new TopicSpecification { Name = topic, NumPartitions = 1 }])
            .Wait();
    }
}