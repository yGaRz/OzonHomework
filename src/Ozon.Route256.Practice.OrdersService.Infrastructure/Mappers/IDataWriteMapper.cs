using Ozon.Route256.Practice.OrdersService.Domain;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Models;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Mappers;

internal interface IDataWriteMapper
{
    OrderDal OrderDalFromDomain(Order order);
}
