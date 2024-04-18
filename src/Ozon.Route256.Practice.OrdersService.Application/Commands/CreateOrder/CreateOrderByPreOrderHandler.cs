using MediatR;
using Ozon.Route256.Practice.OrdersService.Application.Commands.CreateOrder;
using Ozon.Route256.Practice.OrdersService.Application.Mapper;
using Ozon.Route256.Practice.OrdersService.Domain;

namespace Ozon.Route256.Practice.OrdersService.Application.Commands;

public sealed class CreateOrderByPreOrderHandler : IRequestHandler<CreateOrderByPreOrderCommand, Unit>
{
    private readonly IUnitOfCreateOrder _unitOfWork;
    private readonly ICommandMapper _commandMapper;
    public CreateOrderByPreOrderHandler(IUnitOfCreateOrder unitOfWork, ICommandMapper commandMapper)
    {
        _unitOfWork = unitOfWork;
        _commandMapper = commandMapper;
    }

    public async Task<Unit> Handle(CreateOrderByPreOrderCommand request, CancellationToken cancellationToken)
    {
        var preOrderDomain = _commandMapper.PreOrderToDomain(request.PreOrder);
        await _unitOfWork.SaveOrder(preOrderDomain.CreateOrder(), cancellationToken);
        return Unit.Value;
    }
}
