using Bogus;
using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
using Ozon.Route256.Practice.OrdersService.DataAccess.Orders;
using Ozon.Route256.Practice.OrdersService.DataAccess;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.ProduserNewOrder;
using Ozon.Route256.Practice.OrdersService.Models;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Handlers
{
    public class SetOrderStateHandler:ISetOrderStateHandler
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
        public async Task Handle(long id,OrderStateEnum state,DateTime timeUpdate, CancellationToken token)
        {
            try
            {
                await _orderRepository.SetOrderStateAsync(id,state,timeUpdate, token);
                _logger.LogInformation($"{id}");
                //if (GetDistance(order.Address.Latitude, order.Address.Longitude, region.Latitude, region.Longitude) < 5000)
                //{
                //    await _producer.ProduceAsync(new[] { order }, token);
                //    _logger.LogInformation($"Заказ {order.Id} отправлен");
                //}
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
        }
    }
}
