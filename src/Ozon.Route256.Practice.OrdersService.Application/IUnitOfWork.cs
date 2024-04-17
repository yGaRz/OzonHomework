using Ozon.Route256.Practice.OrdersService.Domain;
namespace Ozon.Route256.Practice.OrdersService.Application;
public interface IUnitOfWork
{
    Task SaveOrder(Order order, CancellationToken cancellationToken);
}
