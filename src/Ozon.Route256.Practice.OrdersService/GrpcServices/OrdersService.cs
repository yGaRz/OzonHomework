using Bogus;
using Grpc.Core;
using Ozon.Route256.Practice.LogisticGrpcFile;
using Ozon.Route256.Practice.OrdersGrpcFile;
using Ozon.Route256.Practice.OrdersService.Application;
using Ozon.Route256.Practice.OrdersService.Application.Dto;
using Ozon.Route256.Practice.OrdersService.Domain;
using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.OrdersService.Infrastructure.CacheCustomers;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Models;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Models.Enums;

namespace Ozon.Route256.Practice.OrdersService.GrpcServices;

public sealed class OrdersService: Ozon.Route256.Practice.OrdersGrpcFile.Orders.OrdersBase
{
    private readonly IOrderServiceAdapter _orderServiceAdapter;
    private readonly IGrcpCustomerService _grcpCustomerService;
    public OrdersService( IOrderServiceAdapter orderServiceAdapter,
        IGrcpCustomerService grcpCustomerService)
    {
        _orderServiceAdapter = orderServiceAdapter;
        _grcpCustomerService = grcpCustomerService;
    }

    public override async Task<GetOrderStatusByIdResponse> GetOrderStatusById(GetOrderStatusByIdRequest request, ServerCallContext context)
    {
        return await _orderServiceAdapter.GetOrderByIdAsync(request, context.CancellationToken);
    }
    public override async Task<CancelOrderByIdResponse> CancelOrder(CancelOrderByIdRequest request, ServerCallContext context)
    {
        return await _orderServiceAdapter.CancelOrder(request, context.CancellationToken);
    }        
    public override async Task<GetRegionResponse> GetRegion(GetRegionRequest request, ServerCallContext context)
    {
        var regions = await _orderServiceAdapter.GetRegions(context.CancellationToken);
        var result = new GetRegionResponse
        {
            Region = { regions }
        };
        return result;
    }
    //TODO:4
    public override async Task<GetOrdersResponse> GetOrders(GetOrdersRequest request, ServerCallContext context)
    {
        await Task.Delay(1000);
        //if (!await _regionRepository.IsRegionsExistsAsync(request.Region.ToArray(), context.CancellationToken))
        //    throw new RpcException(new Status(StatusCode.NotFound, "Region not found"));

        //var regions = await _regionRepository.GetRegionsEntityByNameAsync(request.Region.ToArray());

        //var orders = await _ordersRepository.GetOrdersByRegionAsync(regions.Select(x=>x.Name).ToList(), (OrderSourceEnum)request.Source, context.CancellationToken);

        var sortParam = request.SortParam;
        var sortField = request.SortField;
        GetOrdersResponse responce = new GetOrdersResponse();
        //List<OrderDao> result;
        //if (sortField != "" && sortParam != SortParam.None && orders.Length != 0)
        //{
        //    //Type type = orders[0].GetType();
        //    //PropertyInfo? property = type.GetProperty(sortField);
        //    //if (property != null)
        //    //{
        //    //    if (sortParam == SortParam.Asc)
        //    //        result = ReflectionSortHelper.DynamicSort1(orders.ToList(), sortField, "asc");
        //    //    else
        //    //        result = ReflectionSortHelper.DynamicSort1(orders.ToList(), sortField, "desc");
        //    //}
        //    //else
        //    //    throw new RpcException(new Status(StatusCode.Cancelled, $"Sorted field ={sortField} not found"));
        //}
        //else
        //    result = orders.ToList();

        //int page = request.PageIndex-1;
        //int count = request.PageSize;
        //if (result.Count > (page + 1) * count)
        //    responce.Orders.Add(result.GetRange(page * count, count).Select(OrderDao.ConvertToOrderGrpc));
        //else
        //    if(result.Count - page * count>0)
        //        responce.Orders.Add(result.GetRange(page * count, result.Count - page * count).Select(OrderDao.ConvertToOrderGrpc));

        return responce;

    }
    public override async Task<GetOrdersByCustomerIDResponse> GetOrdersByCustomerID(GetOrdersByCustomerIDRequest request, ServerCallContext context)
    {
        try
        {
            return await _orderServiceAdapter.GetOrdersByCustomerID(request, context.CancellationToken);
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
    //TODO:6
    public override async Task<GetRegionStatisticResponse> GetRegionStatistic(GetRegionStatisticRequest request, ServerCallContext context)
    {
        await Task.Delay(1000);
        //if (!await _regionRepository.IsRegionsExistsAsync(request.Region.ToArray(), context.CancellationToken))
        //    throw new RpcException(new Status(StatusCode.NotFound, "Region not found"));

        //RegionStatisticDto[]? result = null;
        //var regions = await _regionRepository.GetRegionsEntityByNameAsync(request.Region.ToArray());
        //result = await _ordersRepository.GetRegionsStatisticAsync(regions.Select(x => x.Name).ToList(), request.StartTime.ToDateTime(), context.CancellationToken);

        GetRegionStatisticResponse regionStatisticResponse = new GetRegionStatisticResponse();
        //foreach ( var item in result )
        //{
        //    regionStatisticResponse.Statistic.Add(new RegionStatisticMessage()
        //    {
        //        Region = item.RegionName,                    
        //        CountCustomers = (int)item.TotalCustomers,
        //        TotalCountOrders= (int)item.TotalCountOrders,
        //        TotalSumOrders= (int)item.TotalSumOrders,
        //        TotalWightOrders= item.TotalWigthOrders
        //    });
        //}
        return regionStatisticResponse;
    }

    //Чтобы можно было сгенерировать пользователей, по факту костыль для тестов.
    public override async Task<GetGenerateCustomerResponse> GetGenerateCustomer(GetGenerateCustomerRequest request, ServerCallContext context)
    {
        for (int i = 1; i <= request.Count; i++)
        {
            Faker faker = new Faker();
            var regionName = await _orderServiceAdapter.GetRegion(faker.Random.Int(1, 3), context.CancellationToken);
            AddressDal address = new AddressDal(regionName.Name,
                                                    faker.Address.City(),
                                                    faker.Address.StreetName(),
                                                    faker.Address.BuildingNumber(),
                                                    faker.Address.StreetSuffix(),
                                                    faker.Address.Latitude(),
                                                    faker.Address.Longitude());
            CustomerDal customer = new CustomerDal()
            {
                Id = i,
                FirstName = faker.Person.FirstName,
                LastName = faker.Person.LastName,
                Phone = faker.Phone.PhoneNumber(),
                Addressed = new AddressDal[1],
                DefaultAddress = address,
                Email = faker.Person.Email
            };
            customer.Addressed[0]=address;
            try
            {
                await _grcpCustomerService.CreateCustomer(customer);
            }
            catch { }
        }
        return new GetGenerateCustomerResponse();
    }
}
