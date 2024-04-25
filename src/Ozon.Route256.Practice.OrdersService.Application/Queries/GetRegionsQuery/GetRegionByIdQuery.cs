using MediatR;
using Ozon.Route256.Practice.OrdersService.Application.Dto;

namespace Ozon.Route256.Practice.OrdersService.Application.Queries.GetRegionsQuery;

public sealed class GetRegionByIdQuery : IRequest<RegionDto>
{
    public int Id { get; init; }
}
