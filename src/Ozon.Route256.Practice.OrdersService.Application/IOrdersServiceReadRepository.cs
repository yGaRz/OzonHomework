using Ozon.Route256.Practice.OrdersService.Application.Dto;
using Ozon.Route256.Practice.OrdersService.Application.Queries.GetOrdersQuery;
using Ozon.Route256.Practice.OrdersService.Application.Queries.GetRegionsQuery;

namespace Ozon.Route256.Practice.OrdersService.Application;
public interface IOrdersServiceReadRepository
{
    Task<OrderDto> GetOrderById(GetOrderByIdQuery request, CancellationToken cancellationToken);
    Task<RegionDto> GetRegionById(GetRegionByIdQuery request, CancellationToken cancellationToken);
    Task<List<RegionDto>> GetRegions(GetRegionsQuery query, CancellationToken token);
}
