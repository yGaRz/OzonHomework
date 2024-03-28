using Bogus;
using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;

namespace Ozon.Route256.Practice.OrdersService.DataAccess.Orders
{
    public class AddOrderHandler : IAddOrderdHandler
    {
        private readonly IOrdersRepository _orderRepository;
        private readonly IRegionRepository _regionRepository;

        public AddOrderHandler(IOrdersRepository orderRepository, IRegionRepository regionRepository)
        {
            _orderRepository = orderRepository;
            _regionRepository = regionRepository;
        }
        public async Task<bool> Handle(OrderEntity order, CancellationToken token)
        {
            try
            {
                order.CustomerId = order.CustomerId%10;
                Faker faker = new Faker(); 
                var region = await _regionRepository.GetRegionEntityById(faker.Random.Int(0, 2));
                order.Address.Region =  region.Name;
                await _orderRepository.CreateOrderAsync(order, token);
                if(GetDistance(order.Address.Latitude, order.Address.Longitude, region.Latitude, region.Longitude) < 5000)
                {
                    //TODO: отправляем в логистику в new_order
                }
            }
            catch (Exception ex)
            {
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
