using Bogus.DataSets;
using Confluent.Kafka;
using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Handlers;
using Ozon.Route256.Practice.OrdersService.Models;
using System.Text.Json;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumers;

public class OrderEventConsumer : ConsumerBackgroundService<long, string>
{
    private readonly ILogger<OrderEventConsumer> _logger;
    private readonly ISetOrderStateHandler _setOrderStateHandler;
    public OrderEventConsumer(
        IServiceProvider serviceProvider,
        IKafkaDataProvider<long, string> kafkaDataProvider,
        ILogger<OrderEventConsumer> logger)
        : base(serviceProvider, kafkaDataProvider, logger)
    {
        _logger = logger;
        _setOrderStateHandler = _scope.ServiceProvider.GetRequiredService<ISetOrderStateHandler>();
        //ConsumerPreOrder = kafkaDataProvider.ConsumerOrderEvent;
        TopicName = "orders_events";
    }

    private record OrderEvent(long Id, OrderStateEnum NewState,DateTime UpdateDate);
    protected override async Task<bool> HandleAsync(ConsumeResult<long, string> message, CancellationToken cancellationToken)
    {
        if (message.Topic == "orders_events")
        {
            var orderEvent = JsonSerializer.Deserialize<OrderEvent>(message.Message.Value);
            await _setOrderStateHandler.Handle(orderEvent.Id, orderEvent.NewState, orderEvent.UpdateDate, cancellationToken);
            _logger.LogInformation($"Заказ Id = {orderEvent.Id} сменил статус на {orderEvent.NewState}");
            return true;
        }
        return false;
    }
}