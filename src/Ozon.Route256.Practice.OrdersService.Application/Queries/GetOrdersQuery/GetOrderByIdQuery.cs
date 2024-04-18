using MediatR;
using Ozon.Route256.Practice.OrdersService.Application.Dto;

namespace Ozon.Route256.Practice.OrdersService.Application.Queries.GetOrdersQuery;

public sealed class GetOrderByIdQuery:IRequest<OrderDto>
{
    public long Id { get; init; }
}
