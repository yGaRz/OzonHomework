using Npgsql;
using Ozon.Route256.Practice.OrdersService.DAL.Common;
using Ozon.Route256.Practice.OrdersService.DAL.Models;

namespace Ozon.Route256.Practice.OrdersService.DAL.Repositories;
public class RegionRepositoryPg : IRegionRepository
{
    private const string Fields = "id, region, latitude, longitude";
    private const string FieldsForInsert = "region, latitude, longitude";
    private const string Table = "regions";

    private readonly IPostgresConnectionFactory _connectionFactory;
    public RegionRepositoryPg(IPostgresConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    public async Task Create(RegionDal regions, CancellationToken token)
    {
        const string sql = @$"
            insert into {Table} ({FieldsForInsert})
            values (:region_name, :latitude, :longitude)
        ";

        await using var connection = _connectionFactory.GetConnection();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.Add("region_name", regions.Name);
        command.Parameters.Add("latitude", regions.Latitude);
        command.Parameters.Add("longitude", regions.Longitude);

        await connection.OpenAsync(token);
        var reader = await command.ExecuteReaderAsync(token);
    }

    public async Task<RegionDal[]> GetAll(CancellationToken token)
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
                    id: reader.GetFieldValue<int>(0),
                    name: reader.GetFieldValue<string>(1),
                    latitude: reader.GetFieldValue<double>(2),
                    longitude: reader.GetFieldValue<double>(3)
                ));
        }

        return result.ToArray();
    }

    private static async Task<int> ReadId(
        NpgsqlDataReader reader,
        CancellationToken token)
    {
        await reader.ReadAsync(token);
        return reader.GetFieldValue<int>(0);
    }
}
