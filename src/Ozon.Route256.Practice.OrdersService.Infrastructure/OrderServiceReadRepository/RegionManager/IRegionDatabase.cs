using Ozon.Route256.Practice.OrdersService.Infrastructure.Models;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.OrderServiceReadRepository.RegionManager;

internal interface IRegionDatabase
{
    Task<Task> CreateRegionAsync(RegionDal region, CancellationToken token = default);
    Task<bool> IsRegionsExistsAsync(string[] regionName, CancellationToken cancellationToken = default);
    Task<RegionDal[]> GetRegionsEntityByIdAsync(int[] regionId, CancellationToken cancellationToken = default);
    Task<RegionDal> GetRegionEntityByIdAsync(int regionId, CancellationToken cancellationToken = default);
    Task<RegionDal[]> GetRegionsEntityByNameAsync(string[] regionName, CancellationToken cancellationToken = default);
    Task<RegionDal> GetRegionEntityByNameAsync(string regionName, CancellationToken cancellationToken = default);
    Task Update(CancellationToken token = default);
}
