using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.ProducerNewOrder.Handlers
{
    public interface IAddOrderHandler
    {
        Task<bool> Handle(OrderDao request, CancellationToken token);
    }
}
