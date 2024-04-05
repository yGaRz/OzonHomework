using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Data;

namespace Ozon.Route256.Practice.OrdersService.DAL.Common;
public interface IPostgresConnectionFactory
{
    NpgsqlConnection GetConnection();
}

public class PostgresConnectionFactory: IPostgresConnectionFactory
{
    private readonly string _connectionString;

    public PostgresConnectionFactory(
        string connectionString)
    {
        _connectionString = connectionString;
    }
    public NpgsqlConnection GetConnection() => new NpgsqlConnection(_connectionString);
}
