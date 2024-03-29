using Confluent.Kafka;
using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
using Ozon.Route256.Practice.OrdersService.DataAccess.Orders;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumers;

public class PreOrderConsumer : ConsumerBackgroundService<long, string>
{
    private readonly ILogger<PreOrderConsumer> _logger;
    private readonly IAddOrderdHandler _addOrderdHandler;
    public PreOrderConsumer(
        IServiceProvider serviceProvider,
        IKafkaDataProvider<long, string> kafkaDataProvider,
        ILogger<PreOrderConsumer> logger)
        : base(serviceProvider, kafkaDataProvider, logger)
    {
        _logger = logger;
        _addOrderdHandler= _scope.ServiceProvider.GetRequiredService<IAddOrderdHandler>();
    }

    protected override async Task HandleAsync(ConsumeResult<long, string> message, CancellationToken cancellationToken)
    {
        var order = new OrderEntity(message.Message.Key,message.Message.Value,message.Message.Timestamp.UtcDateTime);
        await _addOrderdHandler.Handle(order, cancellationToken);
        _logger.LogInformation($"Заказ Id = {order.Id}, Region = {order.Region}, Customer={order.CustomerId}, Source={order.Source} получен.");
    }
}