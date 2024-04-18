
using Ozon.Route256.Practice.OrdersService.Infrastructure.Models;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Models.Enums;
namespace Ozon.Route256.Practice.OrdersService.Infrastructure.OrderServiceReadRepository.Orders;

internal interface IOrdersManager
{
    Task CreateOrderAsync(OrderDal order, CancellationToken token = default);
    Task<OrderDal> GetOrderByIdAsync(long id, CancellationToken token = default);
    Task<bool> SetOrderStateAsync(long id, OrderStateEnum state, DateTime timeUpdate, CancellationToken token = default);
    Task<OrderDal[]> GetOrdersByRegionAsync(List<string> regionList,
                                                        OrderSourceEnum source,
                                                        CancellationToken token = default);
    Task<OrderDal[]> GetOrdersByCutomerAsync(long idCustomer, DateTime dateStart, CancellationToken token = default);
    Task<RegionStatisticDal[]> GetRegionsStatisticAsync(List<string> regionList, DateTime dateStart, CancellationToken token = default);
}

