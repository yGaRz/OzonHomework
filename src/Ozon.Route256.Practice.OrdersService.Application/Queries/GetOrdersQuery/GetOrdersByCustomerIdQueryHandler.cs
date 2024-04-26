using MediatR;
using Ozon.Route256.Practice.OrdersService.Application.Mapper;
using Ozon.Route256.Practice.OrdersService.Domain;
using Ozon.Route256.Practice.OrdersService.Infrastructure.CacheCustomers;

namespace Ozon.Route256.Practice.OrdersService.Application.Queries.GetOrdersQuery;

internal class GetOrdersByCustomerIdQueryHandler : IRequestHandler<GetOrdersByCustomerIdQuery, OrdersByCustomerAggregate>
{
    private readonly IOrdersServiceReadRepository _ordersRepository;
    private readonly ICustomerRepositoryProvider _customerRepository;
    private readonly ICommandMapper _mapper;
    public GetOrdersByCustomerIdQueryHandler(IOrdersServiceReadRepository ordersServiceReadRepository, 
        ICustomerRepositoryProvider cacheCustomers,
        ICommandMapper mapper)
    {
        _ordersRepository = ordersServiceReadRepository;
        _customerRepository = cacheCustomers;
        _mapper = mapper;
    }

    public async Task<OrdersByCustomerAggregate> Handle(GetOrdersByCustomerIdQuery request, CancellationToken cancellationToken)    
    {
        var customerDto = await _customerRepository.GetCustomer(request.Id, cancellationToken);
        var ordersDto = await _ordersRepository.GetOrdersByCustomerId(request,  cancellationToken);        
        var regionsDto = await _ordersRepository.GetRegions(new GetRegionsQuery.GetRegionsQuery(), cancellationToken);

        Customer customer = Customer.CreateInstance(customerDto.Id, 
                                                            customerDto.FirstName, 
                                                            customerDto.LastName,
                                                            customerDto.Phone, 
                                                            Email.CreateInstance(customerDto.Email));

        Address address = Address.CreateInstance(customerDto.DefaultAddress.Region,
            customerDto.DefaultAddress.City,
            customerDto.DefaultAddress.Street,
            customerDto.DefaultAddress.Building,
            customerDto.DefaultAddress.Apartment,
            Coordinates.CreateInstance(customerDto.DefaultAddress.Latitude, customerDto.DefaultAddress.Longitude));
        List<Order> orders= ordersDto.Select(_mapper.OrderToDomain).ToList();

        OrdersByCustomerAggregate ordersByCustomerAggregate = OrdersByCustomerAggregate.CreateInstance(customer, orders, address);
        return ordersByCustomerAggregate;
    }
}
