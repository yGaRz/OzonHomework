using MediatR;
using Ozon.Route256.Practice.OrdersService.Application.Dto;

namespace Ozon.Route256.Practice.OrdersService.Application.Queries.GetOrdersQuery;

internal class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, OrderDto[]>
{

    private readonly IOrdersServiceReadRepository _repository;
    public GetOrdersQueryHandler(IOrdersServiceReadRepository ordersServiceReadRepository)
    {
        _repository= ordersServiceReadRepository;
    }
    public Task<OrderDto[]> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        => _repository.GetOrders(request, cancellationToken);
}
