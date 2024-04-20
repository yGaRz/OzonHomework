using Ozon.Route256.Practice.OrdersService.Application.Commands.KafkaQuery;

namespace Ozon.Route256.Practice.OrdersService.Application;

public interface IKafkaProduceAdapter
{
    Task ProduceAsync(KafkaProduceCommand query, CancellationToken token);
}
