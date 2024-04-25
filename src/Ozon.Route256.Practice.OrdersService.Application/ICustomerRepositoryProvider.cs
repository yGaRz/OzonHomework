using Ozon.Route256.Practice.OrdersService.Application.Dto;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.CacheCustomers;
public interface ICustomerRepositoryProvider
{
    Task<CustomerDto> GetCustomer(int customerId, CancellationToken cancellationToken);
}