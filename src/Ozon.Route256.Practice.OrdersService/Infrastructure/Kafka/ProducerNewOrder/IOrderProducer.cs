using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.ProduserNewOrder;

public interface IOrderProducer
{
    Task ProduceAsync(IReadOnlyCollection<OrderEntity> updatedOrders, CancellationToken token);
}