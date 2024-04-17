using Confluent.Kafka;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.ProducerNewOrder
{
    public interface IKafkaProducer<TKey, TValue>
    {
        public IProducer<TKey, TValue> Producer{ get; }
    }
}
