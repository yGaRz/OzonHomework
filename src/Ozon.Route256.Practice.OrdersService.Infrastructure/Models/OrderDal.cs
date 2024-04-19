using Ozon.Route256.Practice.OrdersService.Infrastructure.Models.Enums;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Models;

internal record OrderDal(
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
