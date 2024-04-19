using Npgsql;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Models.Enums;


namespace Ozon.Route256.Practice.OrdersService.Infrastructure.DAL.Common;

public static class PostgresMapping
{
    [Obsolete]
    public static void MapCompositeTypes()
    {
        var mapper = NpgsqlConnection.GlobalTypeMapper;
        mapper.MapEnum<OrderSourceEnum>("order_source_enum");
        mapper.MapEnum<OrderStateEnum>("order_state_enum");
    }
}