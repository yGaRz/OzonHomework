using Npgsql;
using Ozon.Route256.Practice.OrdersService.DAL.Common;
using Ozon.Route256.Practice.OrdersService.DAL.Models;

namespace Ozon.Route256.Practice.OrdersService.DAL.Repositories;
public class RegionRepositoryPg
{
    private const string Fields = "id, region, latitude, longitude";
    private const string FieldsForInsert = "region, latitude, longitude";
    private const string Table = "regions";

    private readonly IPostgresConnectionFactory _connectionFactory;
    public RegionRepositoryPg(IPostgresConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    public async Task<int[]> Create( RegionDal[] regions, CancellationToken token)
    {
        const string sql = @$"
            insert into {Table} ({FieldsForInsert})
            select {FieldsForInsert} from unnest(:models)
            returning id;
        ";

        await using var connection = _connectionFactory.GetConnection();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add("models", regions);

        await connection.OpenAsync(token);
        var reader = await command.ExecuteReaderAsync(token);
        var result = await ReadIds(reader, token);
        return result;
    }

    public async Task<RegionDal[]> GetAll( CancellationToken token)
    {
        const string sql = @$"
            select {Fields}
            from {Table} ;
        ";

        await using var connection = _connectionFactory.GetConnection();
        await using var command = new NpgsqlCommand(sql, connection);
        //command.Parameters.Add("ids", ids.ToArray());

        await connection.OpenAsync(token);
        await using var reader = await command.ExecuteReaderAsync(token);

        var result = await ReadRegionsDal(reader, token);
        return result.ToArray();
    }

    private static async Task<RegionDal[]> ReadRegionsDal(
    NpgsqlDataReader reader,
    CancellationToken token)
    {
        var result = new List<RegionDal>();
        while (await reader.ReadAsync(token))
        {
            result.Add(
                new RegionDal(
                    Id: reader.GetFieldValue<int>(0),
                    Name: reader.GetFieldValue<string>(1),
                    Latitude: reader.GetFieldValue<double>(2),
                    Longitude: reader.GetFieldValue<double>(3)
                ));
        }

        return result.ToArray();
    }

    private static async Task<int[]> ReadIds(
        NpgsqlDataReader reader,
        CancellationToken token)
    {
        var result = new List<int>();
        while (await reader.ReadAsync(token))
        {
            result.Add(reader.GetFieldValue<int>(0));
        }
        return result.ToArray();
    }
}
