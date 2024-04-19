﻿using Grpc.Core;
using MediatR;
using Ozon.Route256.Practice.OrdersGrpcFile;
using Ozon.Route256.Practice.OrdersService.Application.Commands;
using Ozon.Route256.Practice.OrdersService.Application.Commands.UpdateOrderState;
using Ozon.Route256.Practice.OrdersService.Application.Dto;
using Ozon.Route256.Practice.OrdersService.Application.Queries.GetOrdersQuery;
using Ozon.Route256.Practice.OrdersService.Application.Queries.GetRegionsQuery;
using Ozon.Route256.Practice.OrdersService.Domain.Enums;
using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.OrdersService.Infrastructure.CacheCustomers;
using static Ozon.Route256.Practice.LogisticGrpcFile.LogisticsSimulatorService;

namespace Ozon.Route256.Practice.OrdersService.Application;
public class OrderServiceAdapter : IOrderServiceAdapter
{
    private readonly IMediator _mediator;
    private readonly IContractsMapper _mapper;
    private readonly LogisticsSimulatorServiceClient _logisticsSimulatorServiceClient;

    public OrderServiceAdapter(IMediator mediator, 
        IContractsMapper contractsMapper, 
        LogisticsSimulatorServiceClient logisticsSimulatorServiceClient)
    {
        _mediator = mediator;
        _mapper = contractsMapper;
        _logisticsSimulatorServiceClient = logisticsSimulatorServiceClient;
    }
    public async Task<List<string>> GetRegions(CancellationToken token)
    {
        var regions = await _mediator.Send(new GetRegionsQuery(), token);
        return regions.Select(_mapper.ToContractRegion).ToList();
    }
    public async Task CreateOrder(PreOrderDto preOrder, CancellationToken token)
    {
        await _mediator.Send(new CreateOrderByPreOrderCommand(preOrder), token);
    }
    public async Task<RegionDto> GetRegion(int id, CancellationToken token)
    {
        var query = new GetRegionByIdQuery()
        {
            Id = id
        };
        return await _mediator.Send(query, token);
    }
    public async Task SetOrderStateAsync(long id, OrderStateEnumDomain state, DateTime timeUpdate, CancellationToken token)
    {
        await _mediator.Send(new UpdateOrderStateCommand(id, state,timeUpdate), token);
    }
    public async Task<GetOrderStatusByIdResponse> GetOrderByIdAsync(GetOrderStatusByIdRequest request, CancellationToken token)
    {
        var query = new GetOrderByIdQuery()
        {
            Id = request.Id
        }; 
        var order =  await _mediator.Send(query, token);
        if (order != null)
            return new GetOrderStatusByIdResponse() { LogisticStatus = (OrderState)order.state };
        else
            throw new NotFoundException($"Order by Id = {request.Id} not founded");
    }
    public async Task<CancelOrderByIdResponse> CancelOrder(CancelOrderByIdRequest request, CancellationToken token)
    {
        var query = new GetOrderByIdQuery()
        {
            Id = request.Id
        };
        var order = await _mediator.Send(query, token);
        token.ThrowIfCancellationRequested();
        if (order != null)
        {
            var requestLogistic = new LogisticGrpcFile.Order() { Id = request.Id };
            var responceLogistic = await _logisticsSimulatorServiceClient.OrderCancelAsync(requestLogistic, null, null, token);
            token.ThrowIfCancellationRequested();
            if (responceLogistic.Success)
                return new CancelOrderByIdResponse();
            else
                throw new RpcException(new Status(StatusCode.Cancelled, responceLogistic.Error));
        }
        else
            throw new NotFoundException($"Order by Id = {request.Id} not founded");
    }
    public async Task<GetOrdersByCustomerIDResponse> GetOrdersByCustomerID(GetOrdersByCustomerIDRequest request, CancellationToken token)
    {
        var query = new GetOrdersByIdQuery()
        {
            Id = request.Id,
            StartTime = request.StartTime.ToDateTime(),
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        };
        var ordersByCustomer = await _mediator.Send(query, token);
        GetOrdersByCustomerIDResponse responce = new GetOrdersByCustomerIDResponse
        {
            NameCustomer = ordersByCustomer.CustomerFullName,
            PhoneNumber = ordersByCustomer.CustomerPhone,
            Region = ordersByCustomer.Region,
            AddressCustomer = _mapper.ToContractAddress(ordersByCustomer.Address)
        };
        int page = request.PageIndex - 1;
        int count = request.PageSize;
        var result = ordersByCustomer.Orders.ToList();
        if (result.Count > (page + 1) * count)
            responce.Orders.Add(result.GetRange(page * count, count).Select(_mapper.ToContractOrder));
        else
            if (result.Count - page * count > 0)
            responce.Orders.Add(result.GetRange(page * count, result.Count - page * count).Select(_mapper.ToContractOrder));
        return responce;
    }
    public async Task<GetRegionStatisticResponse> GetRegionStatistic(GetRegionStatisticRequest request, CancellationToken token)
    {
        var regions = await _mediator.Send(new GetRegionsQuery(), token);                
        if( request.Region.Count !=0 && !request.Region.All(x=>regions.Select(r=>r.Name).Contains(x)))
            throw new RpcException(new Status(StatusCode.NotFound, "Region not found"));

        var query = new GetRegionStatisticQuery()
        {
            StartTime  = request.StartTime.ToDateTime()
        };
        query.Regions.AddRange(request.Region);
        RegionStatisticDto[]? result = await _mediator.Send(query, token);
        GetRegionStatisticResponse regionStatisticResponse = new GetRegionStatisticResponse();
        foreach (var item in result)
        {
            regionStatisticResponse.Statistic.Add(new RegionStatisticMessage()
            {
                Region = item.RegionName,
                CountCustomers = (int)item.TotalCustomers,
                TotalCountOrders = (int)item.TotalCountOrders,
                TotalSumOrders = (int)item.TotalSumOrders,
                TotalWightOrders = item.TotalWigthOrders
            });
        }
        return regionStatisticResponse;
    }
}
