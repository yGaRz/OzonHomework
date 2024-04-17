using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.CacheCustomers
{
    public interface IGrcpCustomerService
    {
        public Task<CustomerDto> GetCustomer(int customerId, CancellationToken cancellationToken);
        public Task CreateCustomer(CustomerDto customer, CancellationToken cancellationToken=default);
    }
}
