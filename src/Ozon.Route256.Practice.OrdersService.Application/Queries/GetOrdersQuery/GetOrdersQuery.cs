using MediatR;
using Ozon.Route256.Practice.OrdersService.Application.Dto;
using Ozon.Route256.Practice.OrdersService.Domain.Enums;

namespace Ozon.Route256.Practice.OrdersService.Application.Queries.GetOrdersQuery;

public class GetOrdersQuery : IRequest<OrderDto[]>
{
    public OrderSourceEnumDomain Source {  get; init; }
    public List<string> Regions { get; init; }=new List<string>();
}
