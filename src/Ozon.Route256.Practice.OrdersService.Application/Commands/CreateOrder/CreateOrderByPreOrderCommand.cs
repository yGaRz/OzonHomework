using MediatR;
using Ozon.Route256.Practice.OrdersService.Application.Dto;

namespace Ozon.Route256.Practice.OrdersService.Application.Commands;

public sealed class CreateOrderByPreOrderCommand:IRequest<Unit>
{
    public CreateOrderByPreOrderCommand(PreOrderDto preOrderDto) => PreOrder=preOrderDto;
    public PreOrderDto PreOrder { get; }
}
