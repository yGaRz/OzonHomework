using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.CacheCustomers
{
    public interface IGrcpCustomerService
    {
        public Task<CustomerEntity> GetCustomer(int customerId, CancellationToken cancellationToken);
        public Task CreateCustomer(CustomerEntity customer, CancellationToken cancellationToken=default);
    }
}
