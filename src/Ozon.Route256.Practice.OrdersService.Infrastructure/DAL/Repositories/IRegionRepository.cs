using Ozon.Route256.Practice.OrdersService.Infrastructure.DAL.Models;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.DAL.Repositories
{
    public interface IRegionRepository
    {
        Task Create(RegionDal regions, CancellationToken token);
        Task<RegionDal[]> GetAll(CancellationToken token);
    }
}