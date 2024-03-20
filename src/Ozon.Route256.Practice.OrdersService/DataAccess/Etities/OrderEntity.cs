using Ozon.Route256.Practice.OrdersService.Models;
namespace Ozon.Route256.Practice.OrdersService.DataAccess.Etities;

public record OrderEntity(
    long Id,
    OrderSource Source,
    OrderStateEnum State,
    CustomerEntity Customer,
    IEnumerable<GoodEntity> Goods
);

