using Bogus;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Ozon.Route256.Practice.LogisticGrpcFile;
using Ozon.Route256.Practice.OrdersGrpcFile;
using Ozon.Route256.Practice.CustomerGprcFile;
using Ozon.Route256.Practice.OrdersService.DataAccess;
using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
using Ozon.Route256.Practice.OrdersService.DataAccess.Orders;
using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.OrdersService.Infrastructure.CacheCustomers;
using Ozon.Route256.Practice.OrdersService.Models;
using System.Reflection;
using Type = System.Type;

namespace Ozon.Route256.Practice.OrdersService.GrpcServices;

public sealed class OrdersService: Ozon.Route256.Practice.OrdersGrpcFile.Orders.OrdersBase
{
    public readonly IRegionDatabase _regionRepository;
    public readonly IOrdersManager _ordersRepository;
    public readonly LogisticsSimulatorService.LogisticsSimulatorServiceClient _logisticsSimulatorServiceClient;
    public readonly IGrcpCustomerService _customersClient;

    public OrdersService(IRegionDatabase regionRepository, 
        IOrdersManager ordersRepository, 
        LogisticsSimulatorService.LogisticsSimulatorServiceClient logisticsSimulatorServiceClient,
        IGrcpCustomerService customersClient)
    {
        _regionRepository = regionRepository;
        _ordersRepository = ordersRepository;
        _logisticsSimulatorServiceClient = logisticsSimulatorServiceClient;
        _customersClient = customersClient;
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
            var requestLogistic = new LogisticGrpcFile.Order() { Id = request.Id };
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
        var regions = await _regionRepository.GetRegionsEntityByIdAsync(Array.Empty<int>(), context.CancellationToken);
        var result = new GetRegionResponse
        {
            Region = { regions.Select(x => x.Name).ToArray() }
        };
        return result;
    }
    public override async Task<GetOrdersResponse> GetOrders(GetOrdersRequest request, ServerCallContext context)
    {
        if (!await _regionRepository.IsRegionsExistsAsync(request.Region.ToArray(), context.CancellationToken))
            throw new RpcException(new Status(StatusCode.NotFound, "Region not found"));

        var regions = await _regionRepository.GetRegionsEntityByNameAsync(request.Region.ToArray());
        
        var orders = await _ordersRepository.GetOrdersByRegionAsync(regions.Select(x=>x.Name).ToList(), (OrderSourceEnum)request.Source, context.CancellationToken);

        var sortParam = request.SortParam;
        var sortField = request.SortField;
        GetOrdersResponse responce = new GetOrdersResponse();
        List<OrderEntity> result;
        if (sortField != "" && sortParam != SortParam.None && orders.Length != 0)
        {
            Type type = orders[0].GetType();
            PropertyInfo? property = type.GetProperty(sortField);
            if (property != null)
            {
                if (sortParam == SortParam.Asc)
                    result = ReflectionSortHelper.DynamicSort1(orders.ToList(), sortField, "asc");
                else
                    result = ReflectionSortHelper.DynamicSort1(orders.ToList(), sortField, "desc");
            }
            else
                throw new RpcException(new Status(StatusCode.Cancelled, $"Sorted field ={sortField} not found"));
        }
        else
            result = orders.ToList();

        int page = request.PageIndex-1;
        int count = request.PageSize;
        if (result.Count > (page + 1) * count)
            responce.Orders.Add(result.GetRange(page * count, count).Select(OrderEntity.ConvertToOrderGrpc));
        else
            if(result.Count - page * count>0)
                responce.Orders.Add(result.GetRange(page * count, result.Count - page * count).Select(OrderEntity.ConvertToOrderGrpc));

        return responce;

    }
    public override async Task<GetOrdersByCustomerIDResponse> GetOrdersByCustomerID(GetOrdersByCustomerIDRequest request, ServerCallContext context)
    {
        try
        {
            CustomerEntity customerEntity = await _customersClient.GetCustomer(request.Id, context.CancellationToken);

            var orders = await _ordersRepository.GetOrdersByCutomerAsync(request.Id, request.StartTime.ToDateTime());
            GetOrdersByCustomerIDResponse responce = new GetOrdersByCustomerIDResponse
            {
                NameCustomer = $"{customerEntity.FirstName} {customerEntity.LastName}",
                PhoneNumber = customerEntity.Phone,
                Region = customerEntity.DefaultAddress.Region,
                AddressCustomer = AddressEntity.ConvertToAddressGrpc(customerEntity.DefaultAddress)                    
            };
            int page = request.PageIndex - 1;
            int count = request.PageSize;
            var result = orders.ToList();
            if (result.Count > (page + 1) * count)
                responce.Orders.Add(result.GetRange(page * count, count).Select(OrderEntity.ConvertToOrderGrpc));
            else
                if (result.Count - page * count > 0)
                responce.Orders.Add(result.GetRange(page * count, result.Count - page * count).Select(OrderEntity.ConvertToOrderGrpc));
            return responce;
        }
        catch (RpcException ex)
        {
            if(ex.StatusCode == StatusCode.InvalidArgument)
                throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch(Exception ex)
        {
            throw new RpcException(new Status(StatusCode.Internal, ex.Message));
        }
        throw new RpcException(new Status(StatusCode.Internal, "Эта строчка не должна быть вызвана."));
    }
    public override async Task<GetRegionStatisticResponse> GetRegionStatistic(GetRegionStatisticRequest request, ServerCallContext context)
    {
        if (!await _regionRepository.IsRegionsExistsAsync(request.Region.ToArray(), context.CancellationToken))
            throw new RpcException(new Status(StatusCode.NotFound, "Region not found"));

        RegionStatisticEntity[]? result = null;
        var regions = await _regionRepository.GetRegionsEntityByNameAsync(request.Region.ToArray());
        result = await _ordersRepository.GetRegionsStatisticAsync(regions.Select(x => x.Name).ToList(), request.StartTime.ToDateTime(), context.CancellationToken);
        
        GetRegionStatisticResponse regionStatisticResponse = new GetRegionStatisticResponse();
        foreach ( var item in result )
        {
            regionStatisticResponse.Statistic.Add(new RegionStatisticMessage()
            {
                Region = item.RegionName,                    
                CountCustomers = (int)item.TotalCustomers,
                TotalCountOrders= (int)item.TotalCountOrders,
                TotalSumOrders= (int)item.TotalSumOrders,
                TotalWightOrders= item.TotalWigthOrders
            });
        }
        return regionStatisticResponse;
    }
   public override async Task<GetGenerateCustomerResponse> GetGenerateCustomer(GetGenerateCustomerRequest request, ServerCallContext context)
    {

        for (int i = 1; i <= request.Count; i++)
        {
            Faker faker = new Faker();
            var regionName = await _regionRepository.GetRegionEntityByIdAsync(faker.Random.Int(1, 3),context.CancellationToken);
            AddressEntity address = new AddressEntity(regionName.Name,
                                                    faker.Address.City(),
                                                    faker.Address.StreetName(),
                                                    faker.Address.BuildingNumber(),
                                                    faker.Address.StreetSuffix(),
                                                    faker.Address.Latitude(),
                                                    faker.Address.Longitude());
            CustomerEntity cusromer = new CustomerEntity()
            {
                Id = i,
                DefaultAddress = address,
                Email = faker.Person.Email,
                FirstName = faker.Person.FirstName,
                LastName = faker.Person.LastName,
                Phone = faker.Phone.PhoneNumber()
            };
            try
            {
                await _customersClient.CreateCustomer(cusromer);
            }
            catch { }
        }
        return new GetGenerateCustomerResponse();
    }
}
