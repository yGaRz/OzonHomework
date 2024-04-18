using Ozon.Route256.Practice.OrdersService.Domain.Enums;

namespace Ozon.Route256.Practice.OrdersService.Application.Commands.UpdateOrderState;

public interface IUnitOfUpdateOrder
{
    Task<bool> UpdateOrder(long id, OrderStateEnumDomain state, DateTime timeUpdate, CancellationToken token);
}
