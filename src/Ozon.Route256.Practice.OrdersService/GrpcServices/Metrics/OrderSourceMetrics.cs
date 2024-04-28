using Ozon.Route256.Practice.OrdersService.Application.Metrics;
using Ozon.Route256.Practice.OrdersService.Domain.Enums;
using Prometheus;
namespace Ozon.Route256.Practice.OrdersService.GrpcServices.Metrics;

public class OrderSourceMetrics : IOrderSourceMetrics
{
    private readonly Counter _counter = Prometheus.Metrics.CreateCounter(
        name: "orders_service_create_by_source",
        help: "Создание заказа по источнику",
        "source");
    public void OrderCreated(OrderSourceEnumDomain source)
        => _counter.WithLabels(source.ToString()).Inc();
}
