using Ozon.Route256.Practice.OrdersService.Domain.Enums;
using Prometheus;


namespace Ozon.Route256.Practice.OrdersService.GrpcServices.Metrics;

public class GrpcCounterMetrics
{
    private readonly Counter _counter = Prometheus.Metrics.CreateCounter(
    name: "orders_service_request_by_method",
    help: "Имя метода запроса",
    "methodName");
    public void Request(string name)
        => _counter.WithLabels(name).Inc();
}
