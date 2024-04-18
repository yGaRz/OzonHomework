using Confluent.Kafka;
using Ozon.Route256.Practice.OrdersService.Application.Dto;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.ProducerNewOrder;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.ProduserNewOrder;

internal class OrderProducer : IOrderProducer
{
    private const string TOPIC_NAME = "new_orders";
    private readonly IKafkaProducer<long, string> _kafkaDataProvider;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    public OrderProducer(IKafkaProducer<long, string> kafkaDataProvider)
    {
        _kafkaDataProvider = kafkaDataProvider;
    }

    private record new_order(long OrderId);

    public async Task ProduceAsync(IReadOnlyCollection<OrderDao> updatedOrders, CancellationToken token)
    {
        await Task.Yield();

        var tasks = new List<Task<DeliveryResult<long, string>>>(updatedOrders.Count);

        foreach (var order in updatedOrders)
        {
            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();
            var key = order.Id;
            var value = JsonSerializer.Serialize(new new_order(order.Id), _jsonSerializerOptions);

            var message = new Message<long, string>
            {
                Key = key,
                Value = value
            };
            var task = _kafkaDataProvider.Producer.ProduceAsync(TOPIC_NAME, message, token);
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
    }
}