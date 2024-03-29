using Ozon.Route256.Practice.OrdersService.DataAccess.Etities;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Handlers
{
    public interface IAddOrderHandler
    {
        Task<bool> Handle(OrderEntity request, CancellationToken token);
    }
}
