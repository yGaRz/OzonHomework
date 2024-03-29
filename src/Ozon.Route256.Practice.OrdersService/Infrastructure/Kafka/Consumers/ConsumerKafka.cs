﻿using Bogus.DataSets;
using Confluent.Kafka;
using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Handlers;
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
            var order = new OrderEntity(message.Message.Key,message.Message.Value,message.Message.Timestamp.UtcDateTime);
            await _addOrderdHandler.Handle(order, cancellationToken);
            _logger.LogInformation($"Заказ Id = {order.Id}, Region = {order.Region}, Customer={order.CustomerId}, Source={order.Source} получен.");
        }
        if (message.Topic == "orders_events")
        {
            var orderEvent = JsonSerializer.Deserialize<OrderEvent>(message.Message.Value);
            OrderStateEnum q = orderEvent.OrderState switch
            {
                "Created" => OrderStateEnum.Created,
                "SentToCustomer" => OrderStateEnum.SentToCustomer,
                "Delivered" => OrderStateEnum.Delivered,
                "Lost" => OrderStateEnum.Lost,
                _ => OrderStateEnum.Lost
            };

            await _setOrderStateHandler.Handle(orderEvent.OrderId, q, orderEvent.ChangedAt.DateTime, cancellationToken);
            _logger.LogInformation($"Заказ Id = {orderEvent.OrderId} сменил статус на {orderEvent.OrderState}");
        }
    }
}