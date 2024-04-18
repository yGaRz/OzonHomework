using MediatR;

namespace Ozon.Route256.Practice.OrdersService.Application.Commands.UpdateOrderState;

internal class UpdateOrderStateHandler : IRequestHandler<UpdateOrderStateCommand, bool>
{
    private readonly IUnitOfUpdateOrder _unitOfUpdateOrder;
    public UpdateOrderStateHandler(IUnitOfUpdateOrder unitOfUpdateOrder)
    {
        _unitOfUpdateOrder = unitOfUpdateOrder;
    }

    public async Task<bool> Handle(UpdateOrderStateCommand request, CancellationToken cancellationToken)
    {
        return await _unitOfUpdateOrder.UpdateOrder(request.Id,request.State,request.TimeUpdate,cancellationToken);
    }
}
