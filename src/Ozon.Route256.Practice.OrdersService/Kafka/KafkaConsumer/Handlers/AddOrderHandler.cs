using Bogus;
using MediatR;
using Ozon.Route256.Practice.OrdersService.Application.Dto;
using Ozon.Route256.Practice.OrdersService.DataAccess;
using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
using Ozon.Route256.Practice.OrdersService.DataAccess.Orders;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.ProducerNewOrder.Handlers;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.ProduserNewOrder;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer
{
    public class AddOrderHandler : IAddOrderHandler
    {
        private readonly IOrdersManager _orderManager;
        private readonly IRegionDatabase _regionDatabase;
        private readonly IOrderProducer _producer;
        private readonly ILogger<AddOrderHandler> _logger;

        public AddOrderHandler(IOrdersManager orderManager, IRegionDatabase regionDatabase, IOrderProducer orderProducer, ILogger<AddOrderHandler> logger)
        {
            _orderManager = orderManager;
            _regionDatabase = regionDatabase;
            _producer = orderProducer;
            _logger = logger;
        }
        public async Task<bool> Handle(PreOrderDto order, CancellationToken token)
        {
            try
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();
                order.CustomerId = order.CustomerId % 10 + 1;
                Faker faker = new Faker();
                var region = await _regionDatabase.GetRegionEntityByIdAsync(faker.Random.Int(1, 3));
                order.Address.Region = region.Name;
                await _orderManager.CreateOrderAsync(order, token);
                if (GetDistance(order.Address.Latitude, order.Address.Longitude, region.Latitude, region.Longitude) < 5000)
                {
                    //TODO: await _producer.ProduceAsync(new[] { order }, token);
                    _logger.LogInformation($"Заказ {order.Id} отправлен");
                }
                else
                    _logger.LogInformation($"Заказ {order.Id} не будет отправлен из-за превышения расстояния до склада");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"При обработке заказа {order.Id} возникла ошибка {ex.Message}");
                return false;
            }
            return true;
        }

        private static double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371d;
            var dLat = Deg2Rad(lat2 - lat1);
            var dLon = Deg2Rad(lon2 - lon1);
            var a =
                Math.Sin(dLat / 2d) * Math.Sin(dLat / 2d) +
                Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) *
                Math.Sin(dLon / 2d) * Math.Sin(dLon / 2d);
            var c = 2d * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1d - a));
            var d = R * c;
            return d;
        }
        private static double Deg2Rad(double deg)
        {
            return deg * (Math.PI / 180d);
        }
    }
}
