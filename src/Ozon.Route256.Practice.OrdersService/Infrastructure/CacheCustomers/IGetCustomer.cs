using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.CacheCustomers
{
    public interface IGetCustomer
    {
        public Task<CustomerEntity> GetCustomer(int customerId, CancellationToken cancellationToken);
    }
}
