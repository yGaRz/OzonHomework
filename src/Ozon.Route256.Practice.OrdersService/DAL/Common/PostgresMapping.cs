using Npgsql;
using Ozon.Route256.Practice.OrdersService.Models;


namespace Ozon.Route256.Practice.OrdersService.DAL.Common;

public static class PostgresMapping
{
    public static void MapCompositeTypes()
    {
        var mapper = NpgsqlConnection.GlobalTypeMapper;
        //mapper.MapComposite<AddressDal>("address");
    }
    public static async Task MapEnums(string connectionString)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.MapEnum<OrderSourceEnum>("order_source");
        dataSourceBuilder.MapEnum<OrderStateEnum>("order_state");
        await using var dataSource = dataSourceBuilder.Build();
    }
}