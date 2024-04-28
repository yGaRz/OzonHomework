using MediatR;
using Ozon.Route256.Practice.OrdersService.Application.Commands.CreateOrder;
using Ozon.Route256.Practice.OrdersService.Application.Mapper;
using Ozon.Route256.Practice.OrdersService.Application.Metrics;
using Ozon.Route256.Practice.OrdersService.Domain;

namespace Ozon.Route256.Practice.OrdersService.Application.Commands;

public sealed class CreateOrderByPreOrderHandler : IRequestHandler<CreateOrderByPreOrderCommand, Unit>
{
    private readonly IUnitOfCreateOrder _unitOfWork;
    private readonly ICommandMapper _commandMapper;
    private readonly IOrderSourceMetrics _metrics;
    public CreateOrderByPreOrderHandler(IUnitOfCreateOrder unitOfWork, ICommandMapper commandMapper, IOrderSourceMetrics metrics)
    {
        _unitOfWork = unitOfWork;
        _commandMapper = commandMapper;
        _metrics = metrics;
    }

    public async Task<Unit> Handle(CreateOrderByPreOrderCommand request, CancellationToken cancellationToken)
    {
        var preOrderDomain = _commandMapper.PreOrderToDomain(request.PreOrder);
        var order = preOrderDomain.CreateOrder();
        await _unitOfWork.SaveOrder(order, cancellationToken);
        _metrics.OrderCreated(order.Source);
        return Unit.Value;
    }
}
