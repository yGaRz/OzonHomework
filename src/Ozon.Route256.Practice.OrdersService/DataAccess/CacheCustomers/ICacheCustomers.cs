using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;

namespace Ozon.Route256.Practice.OrdersService.DataAccess.CacheCustomers
{
    public interface ICacheCustomers
    {
        Task<CustomerFullEntity?> Find (int id, CancellationToken cancellationToken);
        Task Insert(CustomerFullEntity customer, CancellationToken cancellationToken);
        Task<bool> IsExists (int id, CancellationToken cancellationToken);
    }
}
