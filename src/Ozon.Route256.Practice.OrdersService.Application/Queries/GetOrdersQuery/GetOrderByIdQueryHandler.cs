using MediatR;
using Ozon.Route256.Practice.OrdersService.Application.Dto;

namespace Ozon.Route256.Practice.OrdersService.Application.Queries.GetOrdersQuery;

internal class GetOrderByIdQueryHandler:IRequestHandler<GetOrderByIdQuery,OrderDto>
{
    private readonly IOrdersServiceReadRepository _repository;
    public GetOrderByIdQueryHandler(IOrdersServiceReadRepository ordersServiceReadRepository)
    {
        _repository = ordersServiceReadRepository;
    }
    public Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        =>_repository.GetOrderById(request,cancellationToken);
}
