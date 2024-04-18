using Ozon.Route256.Practice.OrdersService.Infrastructure.Models;
namespace Ozon.Route256.Practice.OrdersService.Infrastructure.CacheCustomers;
internal interface IGrcpCustomerService
{
    public Task<CustomerDal> GetCustomer(int customerId, CancellationToken cancellationToken);
    public Task CreateCustomer(CustomerDal customer, CancellationToken cancellationToken=default);
}
