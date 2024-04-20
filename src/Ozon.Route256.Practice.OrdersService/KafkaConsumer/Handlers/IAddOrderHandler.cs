using Ozon.Route256.Practice.OrdersService.Application.Dto;
namespace Ozon.Route256.Practice.OrdersService.Kafka.ProducerNewOrder.Handlers;
public interface IAddOrderHandler
{
    Task<bool> Handle(long id, string message, DateTime timeCreate, CancellationToken token);
}
