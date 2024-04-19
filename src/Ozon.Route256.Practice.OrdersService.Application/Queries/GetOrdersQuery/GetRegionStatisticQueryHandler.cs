using MediatR;
using Ozon.Route256.Practice.OrdersService.Application.Dto;

namespace Ozon.Route256.Practice.OrdersService.Application.Queries.GetOrdersQuery;

internal class GetRegionStatisticQueryHandler : IRequestHandler<GetRegionStatisticQuery, RegionStatisticDto[]>
{
    private readonly IOrdersServiceReadRepository _repository;
    public GetRegionStatisticQueryHandler(IOrdersServiceReadRepository repository)
    {
        _repository = repository;
    }

    public Task<RegionStatisticDto[]> Handle(GetRegionStatisticQuery request, CancellationToken cancellationToken)
        =>_repository.GetRegionStatistics(request, cancellationToken);
}
