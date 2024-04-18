using Ozon.Route256.Practice.OrdersService.Application.Commands.UpdateOrderState;
using Ozon.Route256.Practice.OrdersService.Domain.Enums;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Models.Enums;
using Ozon.Route256.Practice.OrdersService.Infrastructure.OrderServiceReadRepository.Orders;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure;

internal class UnitOfUpdateOrder : IUnitOfUpdateOrder
{
    private readonly IOrdersManager _ordersManager;
    public UnitOfUpdateOrder(IOrdersManager ordersManager)
    {
        _ordersManager= ordersManager;
    }
    public async Task<bool> UpdateOrder(long id, OrderStateEnumDomain state, DateTime timeUpdate, CancellationToken token)
    {
       return await _ordersManager.SetOrderStateAsync(id, (OrderStateEnum)state, timeUpdate, token);
    }
}
