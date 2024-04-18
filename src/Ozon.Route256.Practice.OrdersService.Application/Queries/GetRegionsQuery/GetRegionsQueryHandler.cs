using MediatR;
using Ozon.Route256.Practice.OrdersService.Application.Dto;

namespace Ozon.Route256.Practice.OrdersService.Application.Queries.GetRegionsQuery;

internal sealed class GetRegionsQueryHandler : IRequestHandler<GetRegionsQuery, List<RegionDto>>
{
    private readonly IOrdersServiceReadRepository _repository;
    public GetRegionsQueryHandler(IOrdersServiceReadRepository readRepository)
    {
        _repository = readRepository;
    }

    public Task<List<RegionDto>> Handle(GetRegionsQuery request, CancellationToken cancellationToken)
        => _repository.GetRegions(request, cancellationToken);

}
