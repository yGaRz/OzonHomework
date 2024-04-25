using Confluent.Kafka;
using Ozon.Route256.Practice.OrdersService.Application;
using Ozon.Route256.Practice.OrdersService.Application.Commands.KafkaQuery;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.ProducerNewOrder;

internal class OrderProducer : IOrderProducer, IKafkaProduceAdapter
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

    public async Task ProduceAsync(IReadOnlyCollection<long> updatedOrders, CancellationToken token)
    {
        await Task.Yield();

        var tasks = new List<Task<DeliveryResult<long, string>>>(updatedOrders.Count);

        foreach (var order in updatedOrders)
        {
            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();
            var key = order;
            var value = JsonSerializer.Serialize(new new_order(order), _jsonSerializerOptions);

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

    public async Task ProduceAsync(KafkaProduceCommand query, CancellationToken token)
    {
        await ProduceAsync(query.updatedOrders, token);
    }
}