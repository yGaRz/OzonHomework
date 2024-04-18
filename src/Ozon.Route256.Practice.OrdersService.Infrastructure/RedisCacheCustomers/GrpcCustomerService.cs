﻿using Grpc.Core;
using Ozon.Route256.Practice.CustomerGprcFile;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Models;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.CacheCustomers;
//Чтобы можно было сгенерировать пользователей, по факту костыль для тестов.
public class GrpcCustomerService : IGrcpCustomerService
{
    public readonly Customers.CustomersClient _customersClient;
    public readonly ICacheCustomers _customerCache;
    public GrpcCustomerService(Customers.CustomersClient customersClient,
                                    ICacheCustomers customerCache)
    { 
        _customersClient = customersClient;
        _customerCache = customerCache;
    }

    public async Task<CustomerDal> GetCustomer(int customerId, CancellationToken cancellationToken)
    {
        CustomerDal? customerEntity = await _customerCache.Find(customerId, cancellationToken);
        if (customerEntity == null)
        {
            GetCustomerByIdResponse respCustomer = new GetCustomerByIdResponse();
            try
            {
                respCustomer = await _customersClient.GetCustomerByIdAsync(new GetCustomerByIdRequest() { Id = customerId });
            }
            catch (RpcException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"Клиент с id={customerId} не найден"));
            }
            customerEntity = CustomerDal.ConvertFromCustomerGrpc(respCustomer.Customer);
            await _customerCache.Insert(customerEntity, cancellationToken);
        }
        return customerEntity;
    }

    public async Task CreateCustomer(CustomerDal customer, CancellationToken cancellationToken = default)
    {
        CreateCustomerRequest request = new CreateCustomerRequest();
        request.Customer = CustomerDal.ConvertToCustomerGrpc(customer);
        var resp = await _customersClient.CreateCustomerAsync(request);
    }
}
