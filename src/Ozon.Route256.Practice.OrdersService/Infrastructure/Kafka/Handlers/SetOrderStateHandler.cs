using Bogus;
using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
using Ozon.Route256.Practice.OrdersService.DataAccess.Orders;
using Ozon.Route256.Practice.OrdersService.DataAccess;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.ProduserNewOrder;
using Ozon.Route256.Practice.OrdersService.Models;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.ProducerNewOrder.Handlers
{
    public class SetOrderStateHandler : ISetOrderStateHandler
    {
        private readonly IOrdersRepository _orderRepository;
        private readonly IOrderProducer _producer;
        private readonly ILogger<AddOrderHandler> _logger;

        public SetOrderStateHandler(IOrdersRepository orderRepository, IOrderProducer orderProducer, ILogger<AddOrderHandler> logger)
        {
            _orderRepository = orderRepository;
            _producer = orderProducer;
            _logger = logger;
        }
        public async Task Handle(long id, OrderStateEnum state, DateTime timeUpdate, CancellationToken token)
        {
            try
            {
                await _orderRepository.SetOrderStateAsync(id, state, timeUpdate, token);
                _logger.LogInformation($"{id}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
        }
    }
}
