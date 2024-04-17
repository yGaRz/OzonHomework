using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.CacheCustomers
{
    public interface ICacheCustomers
    {
        Task<CustomerDto?> Find(int id, CancellationToken cancellationToken);
        Task Insert(CustomerDto customer, CancellationToken cancellationToken);
        Task<bool> IsExists(int id, CancellationToken cancellationToken);
    }
}
