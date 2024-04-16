namespace Ozon.Route256.Practice.OrdersService.Infrastructure.DAL.Shard.Common;

public interface IShardPostgresConnectionFactory
{
    ShardNpgsqlConnection GetConnectionByBucketId(int bucketId);
    IEnumerable<int> GetAllBuckets();
}
