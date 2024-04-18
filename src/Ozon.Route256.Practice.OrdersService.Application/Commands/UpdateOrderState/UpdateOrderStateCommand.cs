using MediatR;
using Ozon.Route256.Practice.OrdersService.Domain.Enums;

namespace Ozon.Route256.Practice.OrdersService.Application.Commands.UpdateOrderState;

public  class UpdateOrderStateCommand:IRequest<bool>
{
    public UpdateOrderStateCommand(long id, OrderStateEnumDomain state, DateTime timeUpdate)
    {
        Id=id;
        State=state;
        TimeUpdate=timeUpdate;
    }
    public long Id { get; init; }
    public OrderStateEnumDomain State { get;init; }
    public DateTime TimeUpdate { get; init; }
}
