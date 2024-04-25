using Confluent.Kafka;

namespace Ozon.Route256.Practice.OrdersService.Kafka.Consumer;
public class KafkaOrdersEventsProvider : IKafkaConsumer<long, string>
{
    public KafkaOrdersEventsProvider(ILogger<KafkaOrdersEventsProvider> logger, string url_kafka)
    {
        var consumerConfig_order_event = new ConsumerConfig
        {
            GroupId = "order_event_group",
            //BootstrapServers = "broker-1:9091",
            BootstrapServers = url_kafka,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
        };

        ConsumerBuilder<long, string> consumerBuilder_order_event = new(consumerConfig_order_event);
        Consumer = consumerBuilder_order_event
            .SetErrorHandler((_, error) => { logger.LogError(error.Reason); })
            .SetLogHandler((_, message) => logger.LogInformation(message.Message))
            .Build();
    }
    public IConsumer<long, string> Consumer { get; }
}
