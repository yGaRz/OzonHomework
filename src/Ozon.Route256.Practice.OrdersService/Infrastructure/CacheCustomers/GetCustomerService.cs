using Grpc.Core;
using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.CacheCustomers
{
    public class GetCustomerService : IGetCustomer
    {
        public readonly Customers.CustomersClient _customersClient;
        public readonly ICacheCustomers _customerCache;
        public GetCustomerService(Customers.CustomersClient customersClient,
                                        ICacheCustomers customerCache)
        { 
            _customersClient = customersClient;
            _customerCache = customerCache;
        }

        public async Task<CustomerEntity> GetCustomer(int customerId, CancellationToken cancellationToken)
        {
            CustomerEntity? customerEntity = await _customerCache.Find(customerId, cancellationToken);
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
                customerEntity = CustomerEntity.Convert(respCustomer.Customer);
                await _customerCache.Insert(customerEntity, cancellationToken);
            }
            return customerEntity;
        }
    }
}
