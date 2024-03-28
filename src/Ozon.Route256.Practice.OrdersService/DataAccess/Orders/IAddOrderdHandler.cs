using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;

namespace Ozon.Route256.Practice.OrdersService.DataAccess.Orders
{
    public interface IAddOrderdHandler {
        Task<bool> Handle(OrderEntity request, CancellationToken token);
    }
}
