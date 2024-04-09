using Microsoft.Extensions.Options;
using Npgsql;
using Ozon.Route256.Practice.CustomerService.ClientBalancing;

namespace Ozon.Route256.Practice.OrdersService.DAL.Shard.Common;

public class ShardConnectionFactory: IShardPostgresConnectionFactory
{
    private readonly IDbStore _dbStore;
    private DbOptions _dbOptions;

    public ShardConnectionFactory(
        IDbStore dbStore,
        IOptions<DbOptions> dbOptions)
    {
        _dbStore   = dbStore;
        _dbOptions = dbOptions.Value;
    }
    

    public IEnumerable<int> GetAllBuckets()
    {
        for (int bucketId = 0; bucketId < _dbStore.BucketsCount; bucketId++)
        {
            yield return bucketId;
        }
    }

    public ShardNpgsqlConnection GetConnectionByBucketId(
        int bucketId)
    {
        var endpoint = _dbStore.GetEndpointByBucket(bucketId);
        var connectionString = GetConnectionString(endpoint);
        return new ShardNpgsqlConnection(new NpgsqlConnection(connectionString), bucketId);
    }

    private string GetConnectionString(
        DbEndpoint endpoint)
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host     = endpoint.HostAndPort,
            Database = _dbOptions.DatabaseName,
            Username = _dbOptions.User,
            Password = _dbOptions.Password
        };
        return builder.ToString();
    }
}