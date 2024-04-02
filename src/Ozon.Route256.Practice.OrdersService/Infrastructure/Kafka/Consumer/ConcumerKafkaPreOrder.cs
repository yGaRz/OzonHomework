using Confluent.Kafka;
using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.ProducerNewOrder.Handlers;
using System.Text.Json;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer
{
    public class ConcumerKafkaPreOrder : ConsumerBackgroundService<long, string>
    {
        private const string topicName = "pre_orders";
        private readonly ILogger<ConcumerKafkaPreOrder> _logger;
        private readonly IAddOrderHandler _addOrderdHandler;
        public ConcumerKafkaPreOrder(
            IServiceProvider serviceProvider,
            KafkaPreOrderProvider kafkaPreOrderProvider,
            ILogger<ConcumerKafkaPreOrder> logger)
            : base(serviceProvider, kafkaPreOrderProvider, logger, topicName)
        {
            _logger = logger;
            _addOrderdHandler = _scope.ServiceProvider.GetRequiredService<IAddOrderHandler>();
            //_setOrderStateHandler = _scope.ServiceProvider.GetRequiredService<ISetOrderStateHandler>();
        }

        private record OrderEvent(long OrderId, string OrderState, DateTimeOffset ChangedAt);
        protected override async Task HandleAsync(ConsumeResult<long, string> message, CancellationToken cancellationToken)
        {
            var order = new OrderEntity(message.Message.Key, message.Message.Value, message.Message.Timestamp.UtcDateTime);
            var result = await _addOrderdHandler.Handle(order, cancellationToken);
            if (result)
                _logger.LogInformation($"Заказ Id = {order.Id}, Region = {order.Region}, Customer={order.CustomerId}, Source={order.Source} получен.");
        }
    }
}
