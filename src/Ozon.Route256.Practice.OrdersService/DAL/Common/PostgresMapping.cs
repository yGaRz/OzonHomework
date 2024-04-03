using Npgsql;


namespace Ozon.Route256.Practice.OrdersService.DAL.Common;

public static class PostgresMapping
{
    public static void MapCompositeTypes()
    {
        var mapper = NpgsqlConnection.GlobalTypeMapper;
        //mapper.MapComposite<AddressDal>("address");
    }
}