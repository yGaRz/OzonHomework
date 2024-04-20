using Ozon.Route256.Practice.OrdersService.Domain.Enums;

namespace Ozon.Route256.Practice.OrdersService.Kafka.ProducerNewOrder.Handlers;
public interface ISetOrderStateHandler
{
    public Task Handle(long id, OrderStateEnumDomain state, DateTime timeUpdate, CancellationToken token);
}
