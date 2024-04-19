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
    //GrcpCustomerService изолирован от Хоста, и тут используется и вызывается только для генерации customer в сервисе
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
    public override async Task<GetOrdersResponse> GetOrders(GetOrdersRequest request, ServerCallContext context)
    {
        return await _orderServiceAdapter.GetOrders(request, context.CancellationToken);
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
    public override async Task<GetRegionStatisticResponse> GetRegionStatistic(GetRegionStatisticRequest request, ServerCallContext context)
    {
        return await _orderServiceAdapter.GetRegionStatistic(request, context.CancellationToken);
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
