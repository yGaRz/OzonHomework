using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;

namespace Ozon.Route256.Practice.OrdersService.DataAccess
{
    public interface IRegionRepository
    {
        Task<string> GetNameByIdRegionAsync(int id,CancellationToken token=default);
        Task<int> GetIdByRegionNameAsync(string regionName,CancellationToken token=default);
        Task CreateRegionAsync(RegionEntity region, CancellationToken token=default);
        Task<string[]> GetRegionsAsync(CancellationToken token=default);
        Task<bool> IsRegionExist(string regionName, CancellationToken cancellationToken = default);
        Task<bool> IsRegionExists(string[] regionName, CancellationToken cancellationToken = default);
        Task<RegionEntity> GetRegionEntityById(int regionId, CancellationToken cancellationToken = default);
    }
}
