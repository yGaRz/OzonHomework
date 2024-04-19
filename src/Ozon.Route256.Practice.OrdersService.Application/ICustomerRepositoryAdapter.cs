using Ozon.Route256.Practice.OrdersService.Application.Dto;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.CacheCustomers;
public interface ICustomerRepositoryAdapter
{
    Task<CustomerDto> GetCustomer(int customerId, CancellationToken cancellationToken);
}