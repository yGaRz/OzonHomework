using Confluent.Kafka;
using Ozon.Route256.Practice.OrdersService.Kafka.ProducerNewOrder.Handlers;

namespace Ozon.Route256.Practice.OrdersService.Kafka.Consumer
{
    public class ConsumerKafkaPreOrder : ConsumerBackgroundService<long, string>
    {
        private const string topicName = "pre_orders";
        private readonly ILogger<ConsumerKafkaPreOrder> _logger;
        private readonly IAddOrderHandler _addOrderdHandler;
        public ConsumerKafkaPreOrder(
            IServiceProvider serviceProvider,
            KafkaPreOrderProvider kafkaPreOrderProvider,
            ILogger<ConsumerKafkaPreOrder> logger)
            : base(serviceProvider, kafkaPreOrderProvider, logger, topicName)
        {
            _logger = logger;
            _addOrderdHandler = _scope.ServiceProvider.GetRequiredService<IAddOrderHandler>();
        }

        private record OrderEvent(long OrderId, string OrderState, DateTimeOffset ChangedAt);
        protected override async Task HandleAsync(ConsumeResult<long, string> message, CancellationToken cancellationToken)
        {
            var result = await _addOrderdHandler.Handle(message.Message.Key, message.Message.Value, message.Message.Timestamp.UtcDateTime, cancellationToken);
            if (result)
                _logger.LogInformation($"Заказ Id = {message.Message.Key} создан");//, Region = {order.Address.Region}, Customer={order.CustomerId}, Source={order.Source} получен.");
        }
    }
}
