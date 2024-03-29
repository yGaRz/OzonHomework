using Confluent.Kafka;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka;

internal class OrderDataProvider : IKafkaDataProvider<long, string>
{
    public static string Kafka_url { get; set; } = "broker-1:9091";
    public OrderDataProvider(ILogger<OrderDataProvider> logger)
    {
        
        var consumerConfig_pre_order = new ConsumerConfig
        {
            GroupId = "pre_orders_group",
            //BootstrapServers = "broker-1:9091",
            BootstrapServers = Kafka_url,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
        };

        ConsumerBuilder<long, string> consumerBuilder_pre_order = new(consumerConfig_pre_order);
        ConsumerPreOrder = consumerBuilder_pre_order
            .SetErrorHandler((_, error) => { logger.LogError(error.Reason); })
            .SetLogHandler((_, message) => logger.LogInformation(message.Message))
            .Build();

        var consumerConfig_order_event = new ConsumerConfig
        {
            GroupId = "order_event_group",
            //BootstrapServers = "broker-1:9091",
            BootstrapServers = Kafka_url,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
        };

        ConsumerBuilder<long, string> consumerBuilder_order_event = new(consumerConfig_order_event);
        ConsumerOrderEvent = consumerBuilder_order_event
            .SetErrorHandler((_, error) => { logger.LogError(error.Reason); })
            .SetLogHandler((_, message) => logger.LogInformation(message.Message))
            .Build();


        var producerConfig = new ProducerConfig
        {
            //BootstrapServers = "broker-1:9091",
            BootstrapServers = Kafka_url,
        };

        ProducerBuilder<long, string> producerBuilder = new(producerConfig);

        ProducerNewOrder = producerBuilder
            .SetErrorHandler((_, error) => { logger.LogError(error.Reason); })
            .SetLogHandler((_, message) => logger.LogInformation(message.Message))
            .Build();
    }

    public IConsumer<long, string> ConsumerPreOrder { get; }

    public IProducer<long, string> ProducerNewOrder { get; }

    public IConsumer<long, string> ConsumerOrderEvent { get; }
}