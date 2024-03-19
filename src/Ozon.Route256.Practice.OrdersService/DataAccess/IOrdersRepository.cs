using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
namespace Ozon.Route256.Practice.OrdersService.DataAccess
{
    public interface IOrdersRepository
    {
        Task CreateOrderAsync(OrderEntity order, CancellationToken token = default);
        Task<OrderEntity> GetOrderById(int id, CancellationToken token = default);


    }
}
