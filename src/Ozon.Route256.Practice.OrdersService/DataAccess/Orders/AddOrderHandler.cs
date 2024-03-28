using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;

namespace Ozon.Route256.Practice.OrdersService.DataAccess.Orders
{
    public class AddOrderHandler : IAddOrderdHandler
    {
        private readonly IOrdersRepository _orderRepository;
        private readonly ILogger _logger;

        public AddOrderHandler(IOrdersRepository orderRepository, ILogger logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }


        public async Task<bool> Handle(OrderEntity order, CancellationToken token)
        {
            try
            {
                //TODO: сделать конвертацию заказа перед добавлением в БД
                await _orderRepository.CreateOrderAsync(order, token);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Order with id={order.Id} already existing.");
                return false;
            }
            //TODO: после добавления в БД, валидируем и добавляем в new_order


            _logger.LogInformation($"Order with id={order.Id} added.");
            return true;
        }
    }
}
