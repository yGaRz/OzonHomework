using Bogus.DataSets;
using Ozon.Route256.Practice.OrdersService.Models;

namespace Ozon.Route256.Practice.OrdersService.DAL.Models;

public record OrderDal(
    long id,
    int customer_id,
    OrderSourceEnum source,
    OrderStateEnum state,
    DateTime timeCreate,
    DateTime timeUpdate,
    int regioId,
    int countGoods,
    double totalWeigth,
    double totalPrice,
    string addressJson);
