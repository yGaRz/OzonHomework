using Ozon.Route256.Practice.OrdersService.Application;
using Ozon.Route256.Practice.OrdersService.Domain;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Mappers;
using Ozon.Route256.Practice.OrdersService.Infrastructure.OrderServiceReadRepository.Orders;
using Ozon.Route256.Practice.OrdersService.Infrastructure.OrderServiceReadRepository.RegionManager;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Database;

internal sealed class UnitOfCreateOrder : IUnitOfCreateOrder
{
    private readonly IOrdersManager _orderManager;
    private readonly IRegionDatabase _regionDatabase;
    private readonly IDataWriteMapper _mapper;

    public UnitOfCreateOrder(IOrdersManager orderManager, IRegionDatabase regionDatabase, IDataWriteMapper mapper)
    {
        _orderManager = orderManager;
        _regionDatabase = regionDatabase;
        _mapper = mapper;
    }

    public async Task SaveOrder(Order order, CancellationToken cancellationToken)
    {
        await _orderManager.CreateOrderAsync(_mapper.OrderDalFromDomain(order), cancellationToken);
    }
}
