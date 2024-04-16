using NpgsqlTypes;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Models;
public enum OrderSourceEnum
{
    [PgName("WebSite")]
    WebSite = 0,
    [PgName("Mobile")]
    Mobile = 1,
    [PgName("Api")]
    Api = 2
}
