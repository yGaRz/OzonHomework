using Grpc.Core;
using MediatR;
using Ozon.Route256.Practice.OrdersGrpcFile;
using Ozon.Route256.Practice.OrdersService.Application.Commands;
using Ozon.Route256.Practice.OrdersService.Application.Commands.UpdateOrderState;
using Ozon.Route256.Practice.OrdersService.Application.Dto;
using Ozon.Route256.Practice.OrdersService.Application.Queries;
using Ozon.Route256.Practice.OrdersService.Application.Queries.GetOrdersQuery;
using Ozon.Route256.Practice.OrdersService.Application.Queries.GetRegionsQuery;
using Ozon.Route256.Practice.OrdersService.Domain.Enums;
using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Models.Enums;
using static Ozon.Route256.Practice.LogisticGrpcFile.LogisticsSimulatorService;

namespace Ozon.Route256.Practice.OrdersService.Application;
public class OrderServiceAdapter : IOrderServiceAdapter
{
    private readonly IMediator _mediator;
    private readonly IContractsMapper _mapper;
    private readonly LogisticsSimulatorServiceClient _logisticsSimulatorServiceClient;
    public OrderServiceAdapter(IMediator mediator, IContractsMapper contractsMapper, LogisticsSimulatorServiceClient logisticsSimulatorServiceClient)
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
}
