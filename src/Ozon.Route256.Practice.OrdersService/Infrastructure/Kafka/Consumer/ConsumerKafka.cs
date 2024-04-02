using Bogus.DataSets;
using Confluent.Kafka;
using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.ProducerNewOrder.Handlers;
using Ozon.Route256.Practice.OrdersService.Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumers;

public class ConsumerKafka : ConsumerBackgroundService<long, string>
{
    private readonly ILogger<ConsumerKafka> _logger;
    private readonly IAddOrderHandler _addOrderdHandler;
    private readonly ISetOrderStateHandler _setOrderStateHandler;
    public ConsumerKafka(
        IServiceProvider serviceProvider,
        IKafkaDataProvider<long, string> kafkaDataProvider,
        ILogger<ConsumerKafka> logger)
        : base(serviceProvider, kafkaDataProvider, logger)
    {
        _logger = logger;
        _addOrderdHandler= _scope.ServiceProvider.GetRequiredService<IAddOrderHandler>();
        _setOrderStateHandler = _scope.ServiceProvider.GetRequiredService<ISetOrderStateHandler>();
    }

    private record OrderEvent(long OrderId, string OrderState, DateTimeOffset ChangedAt);
    protected override async Task HandleAsync(ConsumeResult<long, string> message, CancellationToken cancellationToken)
    {
        if(message.Topic=="pre_orders")
        {            
            await PreOrderWorkerAsync(message, cancellationToken);
            return;
        }
        if (message.Topic == "orders_events")
        {
            await OrdersEventsWorker(message, cancellationToken);
            return;
        }
    }

    protected async Task PreOrderWorkerAsync(ConsumeResult<long, string> message, CancellationToken cancellationToken)
    {
        var order = new OrderEntity(message.Message.Key, message.Message.Value, message.Message.Timestamp.UtcDateTime);
        var result = await _addOrderdHandler.Handle(order, cancellationToken);
        if(result)
            _logger.LogInformation($"Заказ Id = {order.Id}, Region = {order.Region}, Customer={order.CustomerId}, Source={order.Source} получен.");
    }
    protected async Task OrdersEventsWorker(ConsumeResult<long, string> message, CancellationToken cancellationToken)
    {
        var orderEvent = JsonSerializer.Deserialize<OrderEvent>(message.Message.Value);
        if (orderEvent == null)
            throw new Exception($"Заказ с номером ={message.Message.Key} не получилось десериализовать");
        OrderStateEnum orderStateString = orderEvent.OrderState switch
        {
            "Created" => OrderStateEnum.Created,
            "SentToCustomer" => OrderStateEnum.SentToCustomer,
            "Delivered" => OrderStateEnum.Delivered,
            "Lost" => OrderStateEnum.Lost,
            _ => throw new ArgumentException($"State order id={message.Message.Key} is not correct")
        };

        await _setOrderStateHandler.Handle(orderEvent.OrderId, orderStateString, orderEvent.ChangedAt.DateTime, cancellationToken);
        _logger.LogInformation($"Заказ Id = {orderEvent.OrderId} сменил статус на {orderEvent.OrderState}");
    }
}