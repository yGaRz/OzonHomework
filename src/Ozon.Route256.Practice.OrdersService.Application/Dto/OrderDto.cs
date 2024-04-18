using Ozon.Route256.Practice.OrdersService.Domain.Enums;

namespace Ozon.Route256.Practice.OrdersService.Application.Dto;
public record OrderDto(
    long id,
    int customer_id,
    OrderSourceEnumDomain source,
    OrderStateEnumDomain state,
    DateTime timeCreate,
    DateTime timeUpdate,
    int regionId,
    int countGoods,
    double totalWeigth,
    double totalPrice,
    string addressJson);
