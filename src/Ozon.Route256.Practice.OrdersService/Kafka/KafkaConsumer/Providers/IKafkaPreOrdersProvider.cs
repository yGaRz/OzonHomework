using Confluent.Kafka;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer
{
    public interface IKafkaConsumer<TKey, TValue>
    {
        public IConsumer<TKey, TValue> Consumer { get; }
    }
}
