using Npgsql;
using Ozon.Route256.Practice.OrdersService.DAL.Shard.Common;

namespace Ozon.Route256.Practice.OrdersService.DAL.Repositories.ShardRepository;

public class BaseShardRepository
{
    private readonly IShardPostgresConnectionFactory _connectionFactory;
    private readonly IShardingRule<int> _shardingRule;
    private readonly IShardingRule<string> _stringShardingRule;

    public BaseShardRepository(
        IShardPostgresConnectionFactory connectionFactory,
        IShardingRule<int> shardingRule,
        IShardingRule<string> stringShardingRule)
    {
        _connectionFactory  = connectionFactory;
        _shardingRule       = shardingRule;
        _stringShardingRule = stringShardingRule;
    }

    protected ShardNpgsqlConnection GetConnectionByShardKey(
        int shardKey)
    {
        var bucketId = _shardingRule.GetBucketId(shardKey);
        return _connectionFactory.GetConnectionByBucketId(bucketId);
    }
    
    protected ShardNpgsqlConnection GetConnectionByBucket(
        int bucketId,
        CancellationToken token)
    {
        return _connectionFactory.GetConnectionByBucketId(bucketId);
    }

    protected int GetBucketByShardKey(int shardKey) => 
        _shardingRule.GetBucketId(shardKey);
    
    
    protected ShardNpgsqlConnection GetConnectionBySearchKey(string searchKey)
    {
        var bucketId = _stringShardingRule.GetBucketId(searchKey);
        return _connectionFactory.GetConnectionByBucketId(bucketId);
    }

    protected IEnumerable<int> AllBuckets => _connectionFactory.GetAllBuckets();
}