using Ozon.Route256.Practice.OrdersService.Application.Dto;
using Ozon.Route256.Practice.OrdersService.Infrastructure.CacheCustomers;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Mappers;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.RedisCacheCustomers;

internal class CustomerServiceAdapter : ICustomerRepositoryAdapter
{
    private readonly IGrcpCustomerService _grcpCustomerService;
    private readonly IDataReadMapper _mapper;
    public CustomerServiceAdapter(IGrcpCustomerService grcpCustomerService, IDataReadMapper mapper)
    {
        _grcpCustomerService = grcpCustomerService;
        _mapper = mapper;
    }
    public async Task<CustomerDto> GetCustomer(int customerId, CancellationToken cancellationToken)
    {
        var customer = await _grcpCustomerService.GetCustomer(customerId, cancellationToken);
        return _mapper.CustomerDalToDto(customer);
    }
}
