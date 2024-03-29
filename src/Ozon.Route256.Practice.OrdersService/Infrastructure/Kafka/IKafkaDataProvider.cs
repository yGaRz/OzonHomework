using Confluent.Kafka;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka;

public interface IKafkaDataProvider<TKey, TValue>
{
    public IConsumer<TKey, TValue> ConsumerPreOrder { get; }

    public IConsumer<TKey, TValue> ConsumerOrderEvent { get; }

    public IProducer<TKey, TValue> ProducerNewOrder { get; }

}