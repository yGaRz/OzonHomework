namespace Ozon.Route256.Practice.OrdersService.Application;

public interface IKafkaAdapter
{
    Task ProduceAsync(IReadOnlyCollection<long> updatedOrders, CancellationToken token);
}
