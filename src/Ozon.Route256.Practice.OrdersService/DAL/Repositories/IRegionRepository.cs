using Ozon.Route256.Practice.OrdersService.DAL.Models;

namespace Ozon.Route256.Practice.OrdersService.DAL.Repositories
{
    public interface IRegionRepository
    {
        Task<int> Create(RegionDal regions, CancellationToken token);
        Task<RegionDal[]> GetAll(CancellationToken token);
    }
}