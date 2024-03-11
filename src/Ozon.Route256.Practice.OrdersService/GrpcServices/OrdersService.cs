﻿using Grpc.Core;
using Ozon.Route256.Practice.OrdersService.Exceptions;

namespace Ozon.Route256.Practice.OrdersService.GrpcServices
{
    public sealed class OrdersService:Orders.OrdersBase
    {
        public override Task<CancelOrderByIdResponse> CancelOrder(CancelOrderByIdRequest request, ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Order {request.Id} not found"));
            //throw new NotFoundException($"Order by Id = {request.Id} not founded");
        }
        public override Task<GetOrdersResponse> GetOrders(GetOrdersRequest request, ServerCallContext context)
        {
            return Task.FromResult(new GetOrdersResponse());
        }
        public override Task<GetOrdersByCustomerIDResponse> GetOrdersByCustomerID(GetOrdersByCustomerIDRequest request, ServerCallContext context)
        {
            throw new NotFoundException($"Client with id = {request.Id} not founded");
        }
        public override Task<GetOrdersByRegionResponse> GetOrdersByRegion(GetOrdersByRegionRequest request, ServerCallContext context)
        {
            throw new NotFoundException($"Orders with region = {request.Region} not founded");
        }
        public override Task<GetOrderStatusByIdResponse> GetOrderStatusById(GetOrderStatusByIdRequest request, ServerCallContext context)
        {
            throw new NotFoundException($"Order by Id = {request.Id} not founded");
        }
        public override Task<GetRegionResponse> GetRegion(GetRegionRequest request, ServerCallContext context)
        {
            return Task.FromResult( new GetRegionResponse());
        }
    }

}
