using Confluent.Kafka;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer;

public class KafkaPreOrderProvider:IKafkaConsumer<long, string>
{
    public KafkaPreOrderProvider(ILogger<KafkaPreOrderProvider> logger, string url_kafka)
    {
        var consumerConfig_pre_order = new ConsumerConfig
        {
            GroupId = "pre_orders_group",
            //BootstrapServers = "broker-1:9091",
            BootstrapServers = url_kafka,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
        };

        ConsumerBuilder<long, string> consumerBuilder_pre_order = new(consumerConfig_pre_order);
        Consumer = consumerBuilder_pre_order
            .SetErrorHandler((_, error) => { logger.LogError(error.Reason); })
            .SetLogHandler((_, message) => logger.LogInformation(message.Message))
            .Build();
    }
    public IConsumer<long, string> Consumer { get; }
}
