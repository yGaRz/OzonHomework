using Ozon.Route256.Practice.OrdersService.Application.Dto;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.ProduserNewOrder;

public interface IOrderProducer
{
    Task ProduceAsync(IReadOnlyCollection<OrderDao> updatedOrders, CancellationToken token);
}