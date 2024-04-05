using Bogus.DataSets;
using Ozon.Route256.Practice.OrdersService.Models;

namespace Ozon.Route256.Practice.OrdersService.DAL.Models;

//id serial primary key,
//customer_id int,
//    order_source order_source_enum,
//    order_state order_state_enum,
//    time_create text,
//    time_update text,
//    region_id int,
//    count_goods int,
//    total_weigth numeric,
//    total_price numeric,
//    address jsonb
public record OrderDal(
    long id,
    int customer_id,
    OrderSourceEnum source,
    OrderStateEnum state,
    string dateCreate,
    string dateUpdate,
    int regioId,
    int countGoods,
    double totalWeigth,
    double totalPrice,
    string addressJson);
