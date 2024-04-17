using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;

namespace Ozon.Route256.Practice.OrdersService.DataAccess
{
    public interface IRegionDatabase
    {
        Task<Task> CreateRegionAsync(RegionDto region, CancellationToken token = default);
        Task<bool> IsRegionsExistsAsync(string[] regionName, CancellationToken cancellationToken = default);
        Task<RegionDto[]> GetRegionsEntityByIdAsync(int[] regionId, CancellationToken cancellationToken = default);
        Task<RegionDto> GetRegionEntityByIdAsync(int regionId, CancellationToken cancellationToken = default);
        Task<RegionDto[]> GetRegionsEntityByNameAsync(string[] regionName, CancellationToken cancellationToken = default);
        Task<RegionDto> GetRegionEntityByNameAsync(string regionName, CancellationToken cancellationToken = default);
        Task Update(CancellationToken token = default);
    }
}
