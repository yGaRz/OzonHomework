using MediatR;

namespace Ozon.Route256.Practice.OrdersService.Application.Commands.KafkaQuery;

public class KafkaProduceCommand:IRequest<Unit>
{
    public IReadOnlyCollection<long> updatedOrders = default!;
}
