using Confluent.Kafka;
using Ozon.Route256.Practice.OrdersService.Domain.Enums;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Models.Enums;
using Ozon.Route256.Practice.OrdersService.Kafka.ProducerNewOrder.Handlers;
using System.Text.Json;

namespace Ozon.Route256.Practice.OrdersService.Kafka.Consumer;

public class ConsumerKafkaOrdersEvents : ConsumerBackgroundService<long, string>
{
    private const string topicName = "orders_events";
    private readonly ILogger<ConsumerKafkaOrdersEvents> _logger;
    private readonly ISetOrderStateHandler _setOrderStateHandler;
    public ConsumerKafkaOrdersEvents(
        IServiceProvider serviceProvider,
        KafkaOrdersEventsProvider kafkaOrdersEventsProvider,
        ILogger<ConsumerKafkaOrdersEvents> logger)
        : base(serviceProvider, kafkaOrdersEventsProvider, logger, topicName)
    {
        _logger = logger;
        _setOrderStateHandler = _scope.ServiceProvider.GetRequiredService<ISetOrderStateHandler>();
    }

    private record OrderEvent(long OrderId, string OrderState, DateTimeOffset ChangedAt);
    protected override async Task HandleAsync(ConsumeResult<long, string> message, CancellationToken cancellationToken)
    {
        var orderEvent = JsonSerializer.Deserialize<OrderEvent>(message.Message.Value);
        if (orderEvent == null)
            throw new Exception($"Заказ с номером ={message.Message.Key} не получилось десериализовать");
        OrderStateEnumDomain orderStateString = orderEvent.OrderState switch
        {
            "Created" => OrderStateEnumDomain.Created,
            "SentToCustomer" => OrderStateEnumDomain.SentToCustomer,
            "Delivered" => OrderStateEnumDomain.Delivered,
            "Lost" => OrderStateEnumDomain.Lost,
            "Cancelled" => OrderStateEnumDomain.Cancelled,
            _ => throw new ArgumentException($"State order id={message.Message.Key} is not correct")
        };

        await _setOrderStateHandler.Handle(orderEvent.OrderId, orderStateString, orderEvent.ChangedAt.DateTime, cancellationToken);
        _logger.LogInformation($"Заказ Id = {orderEvent.OrderId} сменил статус на {orderEvent.OrderState}");

    }
}
