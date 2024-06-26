﻿using Azure.Core;
using Grpc.Core;
using Ozon.Route256.Practice.OrdersGrpcFile;
using Ozon.Route256.Practice.OrdersService.Application.Commands;
using Ozon.Route256.Practice.OrdersService.Application.Dto;
using Ozon.Route256.Practice.OrdersService.Domain.Enums;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Models.Enums;

namespace Ozon.Route256.Practice.OrdersService.Application;
public interface IOrderServiceAdapter
{
    Task<List<string>> GetRegions(CancellationToken token);
    Task CreateOrder(PreOrderDto preOrder, CancellationToken token);
    Task<RegionDto> GetRegion(int id, CancellationToken token);
    Task SetOrderStateAsync(long id,OrderStateEnumDomain state,DateTime timeUpdate, CancellationToken token);
    Task<GetOrderStatusByIdResponse> GetOrderByIdAsync(GetOrderStatusByIdRequest request, CancellationToken token);
    Task<CancelOrderByIdResponse> CancelOrder(CancelOrderByIdRequest request, CancellationToken token);
    Task<GetOrdersByCustomerIDResponse> GetOrdersByCustomerID(GetOrdersByCustomerIDRequest request, CancellationToken token);
    Task<GetRegionStatisticResponse> GetRegionStatistic(GetRegionStatisticRequest request, CancellationToken token);
    Task<GetOrdersResponse> GetOrders(GetOrdersRequest request, CancellationToken token);
}
