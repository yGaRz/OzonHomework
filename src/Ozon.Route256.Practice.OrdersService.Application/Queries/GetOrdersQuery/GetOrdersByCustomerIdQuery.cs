using MediatR;
using Ozon.Route256.Practice.OrdersService.Application.Dto;
using Ozon.Route256.Practice.OrdersService.Domain;
using System.Diagnostics.Contracts;

namespace Ozon.Route256.Practice.OrdersService.Application.Queries.GetOrdersQuery;
public class GetOrdersByCustomerIdQuery : IRequest<OrdersByCustomerAggregate>
{
    public int Id { get; init; }
    public DateTime StartTime {  get; init; }
}

