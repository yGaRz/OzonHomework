using MediatR;
using Ozon.Route256.Practice.OrdersService.Application.Mapper;

namespace Ozon.Route256.Practice.OrdersService.Application.Commands;

public sealed class CreateOrderByPreOrderHandler : IRequestHandler<CreateOrderByPreOrderCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICommandMapper _commandMapper;
    public CreateOrderByPreOrderHandler(IUnitOfWork unitOfWork, ICommandMapper commandMapper)
    {
        _unitOfWork = unitOfWork;
        _commandMapper = commandMapper;
    }

    public Task<Unit> Handle(CreateOrderByPreOrderCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
