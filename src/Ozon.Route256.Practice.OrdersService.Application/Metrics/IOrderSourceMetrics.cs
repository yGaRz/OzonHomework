using Ozon.Route256.Practice.OrdersService.Domain.Enums;

namespace Ozon.Route256.Practice.OrdersService.Application.Metrics;

public interface IOrderSourceMetrics
{
    void OrderCreated(OrderSourceEnumDomain source);
}
