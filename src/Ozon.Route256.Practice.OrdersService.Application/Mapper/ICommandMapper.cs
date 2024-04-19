using Ozon.Route256.Practice.OrdersService.Application.Dto;
using Ozon.Route256.Practice.OrdersService.Domain;

namespace Ozon.Route256.Practice.OrdersService.Application.Mapper;
public interface ICommandMapper
{
    PreOrder PreOrderToDomain(PreOrderDto preOrderDto);
    Order OrderToDomain(OrderDto orderDto);

}