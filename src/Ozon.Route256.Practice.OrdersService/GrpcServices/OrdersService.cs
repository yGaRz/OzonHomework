﻿using Bogus;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Ozon.Route256.Practice.LogisticsSimulator.Grpc;
using Ozon.Route256.Practice.OrdersService.DataAccess;
using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;
using Ozon.Route256.Practice.OrdersService.DataAccess.Orders;
using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.OrdersService.Infrastructure.CacheCustomers;
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
        public readonly IGrcpCustomerService _customersClient;

        public OrdersService(IRegionRepository regionRepository, 
            IOrdersRepository ordersRepository, 
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
                        result = ReflectionSortHelper.DynamicSort1(orders.ToList(), sortField, "asc");
                    else
                        result = ReflectionSortHelper.DynamicSort1(orders.ToList(), sortField, "desc");

                    responce.Orders.Add(result.Select(OrderEntity.ConvertToOrderGrpc));
                    return responce;
                }
                else
                    throw new RpcException(new Status(StatusCode.Cancelled, $"Sorted field ={sortField} not found"));
            }

            responce.Orders.Add(orders.Select(OrderEntity.ConvertToOrderGrpc));
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
                foreach (var order in orders)
                    responce.Orders.Add(OrderEntity.ConvertToOrderGrpc(order));
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
                string regionName = await _regionRepository.GetNameByIdRegionAsync(faker.Random.Int(0, 2));
                AddressEntity address = new AddressEntity(regionName,
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

}