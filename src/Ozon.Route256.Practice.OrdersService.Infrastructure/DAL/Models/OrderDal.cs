using Ozon.Route256.Practice.OrdersService.Infrastructure.Models;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.DAL.Models;

public record OrderDal(
    long id,
    int customer_id,
    OrderSourceEnum source,
    OrderStateEnum state,
    DateTime timeCreate,
    DateTime timeUpdate,
    int regionId,
    int countGoods,
    double totalWeigth,
    double totalPrice,
    string addressJson);
