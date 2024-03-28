using Confluent.Kafka;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka;

public interface IKafkaDataProvider<TKey, TValue>
{
    public IConsumer<TKey, TValue> Consumer { get; }

    public IProducer<TKey, TValue> Producer { get; }
}