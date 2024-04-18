using MediatR;
using Ozon.Route256.Practice.OrdersGrpcFile;
using Ozon.Route256.Practice.OrdersService.Application.Commands;
using Ozon.Route256.Practice.OrdersService.Application.Dto;
using Ozon.Route256.Practice.OrdersService.Application.Queries;
using Ozon.Route256.Practice.OrdersService.Application.Queries.GetRegionsQuery;
namespace Ozon.Route256.Practice.OrdersService.Application;
public class OrderServiceAdapter : IOrderServiceAdapter
{
    private readonly IMediator _mediator;
    private readonly IContractsMapper _mapper;

    public OrderServiceAdapter(IMediator mediator, IContractsMapper contractsMapper)
    {
        _mediator = mediator;
        _mapper = contractsMapper;
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
}
