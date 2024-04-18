using MediatR;
using Ozon.Route256.Practice.OrdersService.Application.Dto;

namespace Ozon.Route256.Practice.OrdersService.Application.Queries.GetRegionsQuery;

internal class GetRegionByIdQueryHandler : IRequestHandler<GetRegionByIdQuery, RegionDto>
{
    private readonly IOrdersServiceReadRepository _repository;
    public GetRegionByIdQueryHandler(IOrdersServiceReadRepository repository)
    {
        _repository = repository;
    }

    public Task<RegionDto> Handle(GetRegionByIdQuery request, CancellationToken cancellationToken)
        => _repository.GetRegionById(request, cancellationToken);
}
