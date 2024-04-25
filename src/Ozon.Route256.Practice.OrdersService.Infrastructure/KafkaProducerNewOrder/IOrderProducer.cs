using Ozon.Route256.Practice.OrdersService.Application.Dto;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.ProducerNewOrder;

public interface IOrderProducer
{
    Task ProduceAsync(IReadOnlyCollection<long> updatedOrders, CancellationToken token);
}