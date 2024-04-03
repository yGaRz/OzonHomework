using Confluent.Kafka;
using static Confluent.Kafka.ConfigPropertyNames;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.ProducerNewOrder;

public class KafkaProducerProvider:IKafkaProducer<long,string>
{
    public KafkaProducerProvider(ILogger<KafkaProducerProvider> logger, string url_kafka)
    {
        var producerConfig = new ProducerConfig
        {
            //BootstrapServers = "broker-1:9091",
            BootstrapServers = url_kafka,
        };

        ProducerBuilder<long, string> producerBuilder = new(producerConfig);

        Producer = producerBuilder
            .SetErrorHandler((_, error) => { logger.LogError(error.Reason); })
            .SetLogHandler((_, message) => logger.LogInformation(message.Message))
            .Build();
    }
    public IProducer<long, string> Producer { get; }
}