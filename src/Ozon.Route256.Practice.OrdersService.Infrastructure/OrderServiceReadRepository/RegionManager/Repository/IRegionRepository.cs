using Ozon.Route256.Practice.OrdersService.Infrastructure.Models;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.OrderServiceReadRepository.RegionManager.Repository;

internal interface IRegionRepository
{
    Task Create(RegionDal regions, CancellationToken token);
    Task<RegionDal[]> GetAll(CancellationToken token);
}