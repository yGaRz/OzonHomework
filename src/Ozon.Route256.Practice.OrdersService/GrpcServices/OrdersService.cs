using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Ozon.Route256.Practice.LogisticsSimulator.Grpc;
using Ozon.Route256.Practice.OrdersService.DataAccess;
using Ozon.Route256.Practice.OrdersService.DataAccess.CacheCustomers;
using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
using Ozon.Route256.Practice.OrdersService.DataAccess.Orders;
using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.OrdersService.Models;
using System.Reflection;
using Type = System.Type;

namespace Ozon.Route256.Practice.OrdersService.GrpcServices
{
    public sealed class OrdersService:Orders.OrdersBase
    {
        public readonly IRegionRepository _regionRepository;
        public readonly IOrdersRepository _ordersRepository;
        public readonly LogisticsSimulatorService.LogisticsSimulatorServiceClient _logisticsSimulatorServiceClient;
        public readonly Customers.CustomersClient _customersClient;
        public readonly ICacheCustomers _customerCache;
        public OrdersService(IRegionRepository regionRepository, 
            IOrdersRepository ordersRepository, 
            LogisticsSimulatorService.LogisticsSimulatorServiceClient logisticsSimulatorServiceClient,
            Customers.CustomersClient customersClient,
            RedisCustomerRepository customerCache
            )
        {
            _regionRepository = regionRepository;
            _ordersRepository = ordersRepository;
            _logisticsSimulatorServiceClient = logisticsSimulatorServiceClient;
            _customersClient = customersClient;
            _customerCache = customerCache;
        }

        public override async Task<GetOrderStatusByIdResponse> GetOrderStatusById(GetOrderStatusByIdRequest request, ServerCallContext context)
        {
            var order = await _ordersRepository.GetOrderByIdAsync(request.Id, context.CancellationToken);
            if (order != null)       
                return new GetOrderStatusByIdResponse() { LogisticStatus = (OrderState)order.State };
            else
                throw new NotFoundException($"Order by Id = {request.Id} not founded");
        }
        public override async Task<CancelOrderByIdResponse> CancelOrder(CancelOrderByIdRequest request, ServerCallContext context)
        {
            var id = request.Id;
            var order = await _ordersRepository.GetOrderByIdAsync(id);
            context.CancellationToken.ThrowIfCancellationRequested();
            if (order != null)
            {
                var requestLogistic = new LogisticsSimulator.Grpc.Order() { Id = request.Id };
                var responceLogistic = await _logisticsSimulatorServiceClient.OrderCancelAsync(requestLogistic, null, null, context.CancellationToken);
                context.CancellationToken.ThrowIfCancellationRequested();
                if (responceLogistic.Success)
                    return new CancelOrderByIdResponse();
                else
                    throw new RpcException(new Status(StatusCode.Cancelled, responceLogistic.Error));
            }
            else
                throw new NotFoundException($"Order by Id = {request.Id} not founded");
        }        
        public override async Task<GetRegionResponse> GetRegion(GetRegionRequest request, ServerCallContext context)
        {
            var regions = await _regionRepository.GetRegionsAsync();
            var result = new GetRegionResponse
            {
                Region = { regions.ToArray() }
            };
            return result;
        }
        public override async Task<GetOrdersResponse> GetOrders(GetOrdersRequest request, ServerCallContext context)
        {
            if (!await _regionRepository.IsRegionExists(request.Region.ToArray(), context.CancellationToken))
                throw new RpcException(new Status(StatusCode.NotFound, "Region not found"));

            var orders = request.Region.Count == 0 ?
                    await _ordersRepository.GetOrdersByRegionAsync((await _regionRepository.GetRegionsAsync()).ToList(), (OrderSourceEnum)request.Source, context.CancellationToken)
                : await _ordersRepository.GetOrdersByRegionAsync(request.Region.ToList(), (OrderSourceEnum)request.Source, context.CancellationToken);

            var sortParam = request.SortParam;
            var sortField = request.SortField;
            GetOrdersResponse responce = new GetOrdersResponse();
            if (sortField != "" && sortParam != SortParam.None && orders.Length != 0)
            {
                Type type = orders[0].GetType();
                PropertyInfo? property = type.GetProperty(sortField);
                if (property != null)
                {
                    List<OrderEntity> result;
                    if (sortParam == SortParam.Asc)
                        result = DynamicSort1(orders.ToList(), sortField, "asc");
                    else
                        result = DynamicSort1(orders.ToList(), sortField, "desc");

                    responce.Orders.Add(result.Select(OrderEntity.ConvertOrder));
                    return responce;
                }
                else
                    throw new RpcException(new Status(StatusCode.Cancelled, $"Sorted field ={sortField} not found"));
            }

            responce.Orders.Add(orders.Select(OrderEntity.ConvertOrder));
            return responce;
        }
        public override async Task<GetOrdersByCustomerIDResponse> GetOrdersByCustomerID(GetOrdersByCustomerIDRequest request, ServerCallContext context)
        {
            try
            {
                //TODO:Сходить в Redis за Customer и посмотреть, если не нашел, то сходь в Customer-service  и потом положить в Redis
                CustomerFullEntity? customerFullEntity = await _customerCache.Find(request.Id, context.CancellationToken);
                if(customerFullEntity != null)
                {

                }
                else
                {

                }

                GetCustomerByIdResponse respCustomer = new GetCustomerByIdResponse();
                try
                {
                    respCustomer = await _customersClient.GetCustomerByIdAsync(new GetCustomerByIdRequest() { Id = request.Id });
                }
                catch (RpcException)
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, $"Клиент с id={request.Id} не найден"));
                }
                var orders = await _ordersRepository.GetOrdersByCutomerAsync(request.Id, request.StartTime.ToDateTime());

                GetOrdersByCustomerIDResponse responce = new GetOrdersByCustomerIDResponse
                {
                    NameCustomer = $"{respCustomer.Customer.FirstName} {respCustomer.Customer.LastName}",
                    PhoneNumber = respCustomer.Customer.MobileNumber,
                    Region = respCustomer.Customer.DefaultAddress.Region,
                    AddressCustomer = respCustomer.Customer.DefaultAddress
                };

                foreach (var order in orders) 
                    responce.Orders.Add(OrderEntity.ConvertOrder(order));                    
                return responce;
            }
            catch (RpcException)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Client with id = {request.Id} not founded"));
            }
            throw new RpcException(new Status(StatusCode.NotFound, $"Client with id = {request.Id} not founded"));
        }
        public override async Task<GetRegionStatisticResponse> GetRegionStatistic(GetRegionStatisticRequest request, ServerCallContext context)
        {
            if (!await _regionRepository.IsRegionExists(request.Region.ToArray(), context.CancellationToken))
                throw new RpcException(new Status(StatusCode.NotFound, "Region not found"));

            RegionStatisticEntity[]? result = null;
            if(request.Region.Count==0)
            {
                string[] allRegions = await _regionRepository.GetRegionsAsync(context.CancellationToken);
                result = await _ordersRepository.GetRegionsStatisticAsync(allRegions.ToList(), request.StartTime.ToDateTime(), context.CancellationToken);
            }
            else
                result = await _ordersRepository.GetRegionsStatisticAsync(request.Region.ToList(), request.StartTime.ToDateTime(), context.CancellationToken);
            
            GetRegionStatisticResponse regionStatisticResponse = new GetRegionStatisticResponse();
            foreach ( var item in result )
            {
                regionStatisticResponse.Statistic.Add(new RegionStatisticMessage()
                {
                    Region = item.RegionName,
                    CountCustomer = (int)item.TotalCustomers,
                    TotalCountOrders= (int)item.TotalCountOrders,
                    TotalSumOrders= (int)item.TotalSumOrders,
                    TotalWightOrders= item.TotalWigthOrders
                });
            }
            return regionStatisticResponse;
        }

#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
#pragma warning disable CS8600 // Преобразование литерала, допускающего значение NULL или возможного значения NULL в тип, не допускающий значение NULL.
        private static MethodInfo GetCompareToMethod<T>(T genericInstance, string sortExpression)
        {
            Type genericType = genericInstance.GetType();

            object sortExpressionValue = genericType.GetProperty(sortExpression).GetValue(genericInstance, null);
            Type sortExpressionType = sortExpressionValue.GetType();
            MethodInfo compareToMethodOfSortExpressionType = sortExpressionType.GetMethod("CompareTo", new Type[] { sortExpressionType });
            return compareToMethodOfSortExpressionType;
        }
        private static List<T> DynamicSort1<T>(List<T> genericList, string sortExpression, string sortDirection)
        {
            int sortReverser = sortDirection.ToLower().StartsWith("asc") ? 1 : -1;
            Comparison<T> comparisonDelegate = new Comparison<T>((x, y) =>
            {
                // Just to get the compare method info to compare the values.
                MethodInfo compareToMethod = GetCompareToMethod<T>(x, sortExpression);
                // Getting current object value.
                object xSortExpressionValue = x.GetType().GetProperty(sortExpression).GetValue(x, null);
                // Getting the previous value.
                object ySortExpressionValue = y.GetType().GetProperty(sortExpression).GetValue(y, null);
                // Comparing the current and next object value of collection.
                object result = compareToMethod.Invoke(xSortExpressionValue, new object[] { ySortExpressionValue });
                // Result tells whether the compared object is equal, greater, or lesser.
                return sortReverser * Convert.ToInt16(result);
            });
            // Using the comparison delegate to sort the object by its property.
            genericList.Sort(comparisonDelegate);

            return genericList;
        }
#pragma warning restore CS8600 // Преобразование литерала, допускающего значение NULL или возможного значения NULL в тип, не допускающий значение NULL.
#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.
    }

}
