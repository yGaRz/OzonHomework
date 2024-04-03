using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;

namespace Ozon.Route256.Practice.OrdersService.DataAccess
{
    public interface IRegionRepository
    {
        Task CreateRegionAsync(RegionEntity region, CancellationToken token=default);
        Task<bool> IsRegionExistsAsync(string[] regionName, CancellationToken cancellationToken = default);
        Task<RegionEntity> GetRegionsEntityByIdAsync(int[] regionId, CancellationToken cancellationToken = default);
        Task<RegionEntity> GetRegionsEntityByNameAsync(string[] regionName, CancellationToken cancellationToken = default);
    }
}
