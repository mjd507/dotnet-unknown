namespace DotNetUnknown.Kafka;

public static class KafkaExtension
{
    public static void AddKafka(this IServiceCollection services)
    {
        services
            .AddOptions<KafkaOptions>().BindConfiguration("Kafka");
        services
            .AddSingleton<KafkaConfig>()
            .AddSingleton<KafkaService>()
            .AddHostedService<KafkaService.KafkaBackgroundService>();
    }
}