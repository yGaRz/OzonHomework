using Confluent.Kafka;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka;

internal class OrderDataProvider : IKafkaDataProvider<long, string>
{
    public OrderDataProvider(ILogger<OrderDataProvider> logger)
    {
        var consumerConfig = new ConsumerConfig
        {
            GroupId = "new_orders_group",
            BootstrapServers = "localhost:29091",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
        };

        ConsumerBuilder<long, string> consumerBuilder = new(consumerConfig);
        Consumer = consumerBuilder
            .SetErrorHandler((_, error) => { logger.LogError(error.Reason); })
            .SetLogHandler((_, message) => logger.LogInformation(message.Message))
            .Build();

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = "localhost:29092",
        };

        ProducerBuilder<long, string> producerBuilder = new(producerConfig);

        Producer = producerBuilder
            .SetErrorHandler((_, error) => { logger.LogError(error.Reason); })
            .SetLogHandler((_, message) => logger.LogInformation(message.Message))
            .Build();
    }

    public IConsumer<long, string> Consumer { get; }

    public IProducer<long, string> Producer { get; }
}