using NpgsqlTypes;

namespace Ozon.Route256.Practice.OrdersService.Models;
public enum OrderStateEnum
{
    [PgName("Created")]
    Created = 0,
    [PgName("SentToCustomer")]
    SentToCustomer = 1,
    [PgName("Delivered")]
    Delivered = 2,
    [PgName("Lost")]
    Lost = 3,
    [PgName("Cancelled")]
    Cancelled = 4
}
