using Ozon.Route256.Practice.OrdersService.Infrastructure.Models;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.ProducerNewOrder.Handlers
{
    public interface ISetOrderStateHandler
    {
        public Task Handle(long id, OrderStateEnum state, DateTime timeUpdate, CancellationToken token);
    }
}
